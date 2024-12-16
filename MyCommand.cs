using ErrorOr;
using MediatR;

namespace dnet_fluent_erroror
{
    public class MyCommand : IRequest<ErrorOr<SomeResult>>
    {
        public int SomeNumber { get; set; }
    }
}