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
                     // Works: You can create an ErrorOr<T> from any Error given you specify T 
                     //return (TResponse)(object)CommonError.Validator.ToErrorOr<SomeResult>();
                     
                     
                     // Fails: Calling ToErrorOr<T> on an Error with dynamic reflected T
                     var t = GetInnerGenericType(typeof(TResponse));
                     Console.WriteLine(t.Name); // -> SomeResult

                     Console.WriteLine("1");
                     var method = typeof(ErrorOrExtensions).GetMethods( BindingFlags.Public | BindingFlags.Static).Single( m => m.Name == nameof(ErrorOrExtensions.ToErrorOr) && m.GetParameters().Single().ParameterType == typeof(Error));
                     Console.WriteLine("2");
                     var genMethod = method.MakeGenericMethod(t);
                     Console.WriteLine("3");
                     // Error:  System.ArgumentException: Object of type 'ErrorOr.Error' cannot be converted to type 'dnet_fluent_erroror.SomeResult'.
                     var invokeRes = genMethod.Invoke(null, new object[] { CommonError.Validator });
                     Console.WriteLine("4");
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

        static Type GetInnerGenericType(Type type)
        {
            
            // Get the generic argument of IRequest<T>
            var outerGenericArgument = type.GetGenericArguments()[0];
            return outerGenericArgument;
            
        }
    }
}