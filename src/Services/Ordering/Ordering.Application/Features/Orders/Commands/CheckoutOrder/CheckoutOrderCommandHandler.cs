using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder;

public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly ILogger<CheckoutOrderCommandHandler> _logger;

    public CheckoutOrderCommandHandler(
        IOrderRepository orderRepository,
        IMapper mapper,
        IEmailService emailService,
        ILogger<CheckoutOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        var orderEntity = _mapper.Map<Order>(request);
        var newOrder = await _orderRepository.AddAsync(orderEntity);

        _logger.LogInformation("Order {OrderId} is successfully created.", newOrder.Id);

        await SendMail(newOrder);

        return newOrder.Id;
    }

    private async Task SendMail(Order order)
    {
        var email = new Email
        {
            To = order.EmailAddress,
            Body = $"Order {order.Id} was placed successfully. Total: {order.TotalPrice:C}",
            Subject = "Order Placed Successfully"
        };

        try
        {
            await _emailService.SendEmail(email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Order {OrderId} failed to send confirmation email due to: {Error}", order.Id, ex.Message);
        }
    }
}
