using Application.Behaviors;
using Application.Events;
using Application.Interfaces.Services;
using Domain.Events;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);


            //events
            services.AddScoped<IEventHandler<SupportAgentAssignedDomainEvent>, SupportAgentAssignedDomainEventHandler>();
            services.AddScoped<IEventHandler<TicketResolvedDomainEvent>, TicketResolvedDomainEventHandler>();

            return services;
        }
    }
}
