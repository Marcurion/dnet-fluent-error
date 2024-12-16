using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using MediatR;

namespace dnet_fluent_erroror
{
    public class MyCommandHandler : IRequestHandler<MyCommand, ErrorOr<SomeResult>>
    {
        public async Task<ErrorOr<SomeResult>> Handle(MyCommand request, CancellationToken cancellationToken)
        {
            return new SomeResult() { SomeOtherNumber = (uint)request.SomeNumber };
        }
    }
}