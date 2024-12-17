using FluentValidation;

namespace dnet_fluent_erroror
{
    public class MyCommandValidator : AbstractValidator<MyCommand>
    {

        public MyCommandValidator()
        {
            RuleFor(x => x.SomeNumber).GreaterThanOrEqualTo(0).WithMessage("Number must not be negative");
            RuleFor(x => x.SomeNumber).GreaterThan(5).WithMessage("Number must be greater than five");

            RuleFor(x => x.Message).NotNull().NotEmpty().WithMessage("Message must be populated");
        }
    }
}