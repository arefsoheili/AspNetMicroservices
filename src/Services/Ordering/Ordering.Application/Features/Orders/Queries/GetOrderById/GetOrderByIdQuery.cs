using MediatR;
using Ordering.Application.Features.Orders.Queries.GetOrdersList;

namespace Ordering.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQuery : IRequest<OrdersVm>
{
    public int Id { get; }

    public GetOrderByIdQuery(int id)
    {
        Id = id;
    }
}
