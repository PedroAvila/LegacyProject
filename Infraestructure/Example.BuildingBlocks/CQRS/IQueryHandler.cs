using PAN.Mediator.Abstractions;

namespace Example.BuildingBlocks.CQRS
{
    public interface IQueryHandler<in TQuery, TResponse> : IPanRequestHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
    }
}
