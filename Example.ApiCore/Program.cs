using Example.Adapters;
using Example.BuildingBlocks.Behaviors;
using Example.BuildingBlocks.Exceptions.Handler;
using Example.Ports;
using Example.UseCase.Features.Customers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PAN.Mediator.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PruebaDb");

builder.Services.AddPanMediator(config =>
{
    config.RegisterServicesFromAssembly(typeof(CreateCustomerUseCaseHandler).Assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomerCommandValidator>();

builder.Services.AddScoped<ICustomerRepository>(_ => new CustomerRepository(connectionString));

builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        var request = context.HttpContext.Request;

        var errores = context
            .ModelState.Where(e => e.Value?.Errors.Count > 0)
            .Select(e =>
                $"{e.Key}: {string.Join(", ", e.Value!.Errors.Select(x => x.ErrorMessage))}"
            );

        logger.LogWarning(
            "Validación fallida | {Method} {Path} | Errores: {Errores}",
            request.Method,
            request.Path,
            string.Join(" | ", errores)
        );

        return new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
