namespace dnet_fluent_erroror
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using ErrorOr;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;

    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);

            var validationFailures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(failure => failure != null)
                .ToList();



             if (validationFailures.Any())
             {
                 if (UsesErrorOr(typeof(TResponse)))
                 {
                     var t = typeof(TResponse).GetGenericArguments()[0];
                     
                     var method = typeof(ErrorOrExtensions).GetMethods( BindingFlags.Public | BindingFlags.Static).Single( m => m.Name == nameof(ErrorOrExtensions.ToErrorOr) && m.GetParameters().Single().ParameterType == typeof(List<Error>));
                     var genMethod = method.MakeGenericMethod(t);
                     
                     List<Error> errList = new List<Error>() { CommonError.Validator };
                     foreach (var failure in validationFailures)
                     {
                         errList.Add(Error.Validation(code: $"Validation.{t.Name}.{failure.PropertyName}.{failure.ErrorCode}", description: failure.ErrorMessage));
                     }
                     
                     var invokeRes = genMethod.Invoke(null, new object[] { errList });
                     return (TResponse)invokeRes;
                     
                 }
                 else
                 {
                     // Handling for commands which do not support ErrorOr
                     throw new ValidationException(validationFailures);
                 }
             }

            return await next();
        }

        private bool UsesErrorOr(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ErrorOr<>);
        }
        
    }
}