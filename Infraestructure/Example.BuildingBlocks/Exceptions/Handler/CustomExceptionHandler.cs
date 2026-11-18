using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace Example.BuildingBlocks.Exceptions.Handler
{
    /// <summary>
    /// Handler global de excepciones de ASP.NET Web API 2. Traduce ValidationException
    /// y BusinessException a una respuesta ProblemDetails, y oculta el detalle de
    /// cualquier error inesperado.
    /// </summary>
    public sealed class CustomExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<CustomExceptionHandler> _logger;

        public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var exception = context.Exception;
            var correlationId = request.GetCorrelationId();

            _logger.LogError(
                exception,
                "Error Message: {exceptionMessage}, Time of occurrence {time}, Request {method} {path}, TraceId {traceId}",
                exception.Message,
                DateTime.UtcNow,
                request.Method,
                request.RequestUri?.AbsolutePath,
                correlationId);

            var validationException = exception as ValidationException;
            var businessException = exception as BusinessException;

            string title;
            string detail;
            HttpStatusCode statusCode;

            if (validationException != null)
            {
                title = validationException.GetType().Name;
                detail = validationException.Message;
                statusCode = HttpStatusCode.BadRequest;
            }
            else if (businessException != null)
            {
                title = "Error de negocio";
                detail = businessException.Message;
                // BusinessException deja Status en 0 cuando se usa el constructor simple.
                statusCode = businessException.Status > 0
                    ? (HttpStatusCode)businessException.Status
                    : HttpStatusCode.BadRequest;
            }
            else
            {
                title = "Internal Server Error";
                detail = "Ocurrió un error inesperado en nuestro sistema. Por favor, intente más tarde.";
                statusCode = HttpStatusCode.InternalServerError;
            }

            var problemDetails = new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = (int)statusCode,
                Instance = request.RequestUri?.AbsolutePath,
                TraceId = correlationId.ToString()
            };

            if (validationException != null)
            {
                problemDetails.ValidationErrors = validationException.Errors
                    .Select(failure => (object)new
                    {
                        failure.PropertyName,
                        failure.ErrorMessage
                    })
                    .ToList();
            }

            context.Result = new ResponseMessageResult(
                request.CreateResponse(statusCode, problemDetails));

            return Task.CompletedTask;
        }
    }
}
