using PAN.Mediator.Abstractions;

namespace Example.BuildingBlocks.CQRS
{
    /// <summary>
    /// Consulta: operacion de solo lectura que devuelve un resultado.
    /// </summary>
    public interface IQuery<out TResponse> : IPanRequest<TResponse>
    {
    }
}
