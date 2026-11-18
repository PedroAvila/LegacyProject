using Example.Adapters;
using Example.BuildingBlocks.Behaviors;
using Example.BuildingBlocks.Exceptions.Handler;
using Example.BuildingBlocks.Logging;
using Example.Ports;
using Example.UseCase.Features.Customers;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PAN.Mediator.DependencyInjection;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ExceptionHandling;

namespace Example.Api
{
    public static class MediatorConfig
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static void Register()
        {
            var services = new ServiceCollection();

            services.AddSingleton(typeof(ILogger<>), typeof(TraceLogger<>));
            services.AddSingleton<CustomExceptionHandler>();

            services.AddPanMediator(config =>
            {
                config.RegisterServicesFromAssembly(typeof(WebApiApplication).Assembly);
                config.RegisterServicesFromAssembly(typeof(CreateCustomerUseCaseHandler).Assembly);

                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });

            RegisterValidators(services);

            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                var connectionStringSettings = ConfigurationManager.ConnectionStrings["PruebaDb"];
                connectionString = connectionStringSettings?.ConnectionString;
            }

            services.AddTransient<ICustomerRepository>(
                _ => new CustomerRepository(connectionString));

            foreach (var controllerType in typeof(WebApiApplication).Assembly.GetTypes()
                .Where(type => !type.IsAbstract && typeof(IHttpController).IsAssignableFrom(type)))
            {
                services.AddTransient(controllerType);
            }

            ServiceProvider = services.BuildServiceProvider();

            GlobalConfiguration.Configuration.DependencyResolver =
                new WebApiDependencyResolver(ServiceProvider);

            // Reemplaza el handler por defecto de Web API por el que devuelve ProblemDetails.
            GlobalConfiguration.Configuration.Services.Replace(
                typeof(IExceptionHandler),
                ServiceProvider.GetRequiredService<CustomExceptionHandler>());
        }

        private static void RegisterValidators(IServiceCollection services)
        {
            foreach (var validatorType in typeof(CreateCustomerUseCaseHandler).Assembly.GetTypes()
                .Where(type => !type.IsAbstract && !type.IsInterface))
            {
                var validatorInterfaces = validatorType.GetInterfaces()
                    .Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IValidator<>));

                foreach (var validatorInterface in validatorInterfaces)
                {
                    services.AddTransient(validatorInterface, validatorType);
                }
            }
        }
    }
}
