using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;

namespace Ordering.Api.Consumers;

public class BasketCheckoutConsumer : IConsumer<BasketCheckoutIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<BasketCheckoutConsumer> _logger;

    public BasketCheckoutConsumer(IMediator mediator, IMapper mapper, ILogger<BasketCheckoutConsumer> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BasketCheckoutIntegrationEvent> context)
    {
        var command = _mapper.Map<CheckoutOrderCommand>(context.Message);
        var orderId = await _mediator.Send(command);
        _logger.LogInformation("Created order {OrderId} from basket checkout for {UserName}", orderId, context.Message.UserName);
    }
}
