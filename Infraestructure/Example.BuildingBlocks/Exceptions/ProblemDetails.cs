using System.Collections.Generic;

namespace Example.BuildingBlocks.Exceptions
{
    /// <summary>
    /// Equivalente mínimo de ProblemDetails para .NET Framework, ya que
    /// Microsoft.AspNetCore.Mvc.ProblemDetails no existe en este proyecto.
    /// Se serializa en camelCase por el CamelCasePropertyNamesContractResolver
    /// configurado en WebApiConfig.
    /// </summary>
    public sealed class ProblemDetails
    {
        public string Title { get; set; }

        public string Detail { get; set; }

        public int? Status { get; set; }

        public string Instance { get; set; }

        public string TraceId { get; set; }

        public IList<object> ValidationErrors { get; set; }
    }
}
