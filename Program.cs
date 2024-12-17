using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace dnet_fluent_erroror
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            IMediator mediator;
            
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MyCommandHandler).Assembly));
                    services.AddValidatorsFromAssembly(typeof(MyCommandValidator).Assembly, ServiceLifetime.Transient);
                    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                }).Build();

            mediator = host.Services.GetRequiredService<IMediator>();

            var mediatorResult = await mediator.Send(new MyCommand() { SomeNumber = -1 });

            if (mediatorResult.IsError)
            {
                Console.WriteLine($"Error #({mediatorResult.Errors.Count}|{mediatorResult.Errors.Count}) of the MediatR command: {mediatorResult.Errors.Last().Code}-{mediatorResult.Errors.Last().Description}");
            }
            else
            {
                Console.WriteLine($"Result of the MediatR command: {mediatorResult.Value.SomeOtherNumber}");
            }
            

            await host.RunAsync();
            
        }
    }
}