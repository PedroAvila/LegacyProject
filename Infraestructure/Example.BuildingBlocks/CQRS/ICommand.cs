using PAN.Mediator.Abstractions;

namespace Example.BuildingBlocks.CQRS
{
    /// <summary>
    /// BuildingBlocks es un proyecto Bloque de Construccion donde se crea la abstraccion
    /// de PAN.Mediator para los comandos (operaciones que modifican estado).
    /// </summary>

    public interface ICommand : ICommand<Unit>
    {
    }

    public interface ICommand<out TResponse> : IPanRequest<TResponse>
    {
    }
}
