using FluentValidation;

namespace dnet_fluent_erroror
{
    public class MyCommandValidator : AbstractValidator<MyCommand>
    {

        public MyCommandValidator()
        {
            RuleFor(x => x.SomeNumber).GreaterThanOrEqualTo(0).WithMessage("Number must not be negative");
        }
    }
}