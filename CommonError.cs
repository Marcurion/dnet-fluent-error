using ErrorOr;

namespace dnet_fluent_erroror
{
    public static class CommonError
    {
        public static Error Validator = Error.Validation(
            code: "CommonErrors.Validation",
            description: "The request does not comply with the specifications"
        );
    }
}