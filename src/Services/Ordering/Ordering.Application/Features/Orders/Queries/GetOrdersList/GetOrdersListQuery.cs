using MediatR;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList;

public class GetOrdersListQuery : IRequest<List<OrdersVm>>
{
    public string UserName { get; }

    public GetOrdersListQuery(string userName)
    {
        UserName = userName;
    }
}
