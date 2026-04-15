using MediatR;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder;

public class DeleteOrderCommand : IRequest
{
    public int Id { get; }

    public DeleteOrderCommand(int id)
    {
        Id = id;
    }
}
