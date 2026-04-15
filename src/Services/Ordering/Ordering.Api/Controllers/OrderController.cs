using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using Ordering.Application.Features.Orders.Commands.DeleteOrder;
using Ordering.Application.Features.Orders.Commands.UpdateOrder;
using Ordering.Application.Features.Orders.Queries.GetOrderById;
using Ordering.Application.Features.Orders.Queries.GetOrdersList;
using System.Net;

namespace Ordering.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrdersVm), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<OrdersVm>> GetOrderById(int id)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(id));
        return Ok(order);
    }

    [HttpGet("by-user/{userName}")]
    [ProducesResponseType(typeof(List<OrdersVm>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<List<OrdersVm>>> GetOrdersByUserName(string userName)
    {
        var query = new GetOrdersListQuery(userName);
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<int>> CheckoutOrder([FromBody] CheckoutOrderCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        await _mediator.Send(new DeleteOrderCommand(id));
        return NoContent();
    }
}
