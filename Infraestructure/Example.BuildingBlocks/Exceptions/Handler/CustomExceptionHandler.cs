using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Example.BuildingBlocks.Exceptions.Handler
{
    public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
    {

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            logger.LogError(
                exception,
                "Error Message: {exceptionMessage}, Time of occurrence {time}, Request {method} {path}, TraceId {traceId}",
                exception.Message,
                DateTime.UtcNow,
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            (string Detail, string Title, int StatusCode) = exception switch
            {
                ValidationException => (
                    exception.Message,
                    exception.GetType().Name,
                    StatusCodes.Status400BadRequest
                ),
                BusinessException customException => (
                    customException.Message,
                    "Error de negocio",
                    (int)customException.Status
                ),
                _ => (
                    "Ocurrió un error inesperado en nuestro sistema. Por favor, intente más tarde.",
                    "Internal Server Error",
                    StatusCodes.Status500InternalServerError
                ),
            };

            context.Response.StatusCode = StatusCode;

            var problemDetails = new ProblemDetails
            {
                Title = Title,
                Detail = Detail,
                Status = StatusCode,
                Instance = context.Request.Path,
            };

            problemDetails.Extensions.Add("traceId", context.TraceIdentifier);
            if (exception is ValidationException validationException)
                problemDetails.Extensions.Add("ValidationErrors", validationException.Errors);

            await context.Response.WriteAsJsonAsync(
                problemDetails,
                cancellationToken: cancellationToken
            );
            return true;
        }
    }
}
