using Basket.Api.Entities;
using Basket.Api.GrpcService;
using Basket.Api.Repositores;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Basket.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly DiscountGrpcService _discountGrpcService;
        private readonly IPublishEndpoint _publishEndpoint;

        public BasketController(IBasketRepository basketRepository, DiscountGrpcService discountGrpcService, IPublishEndpoint publishEndpoint)
        {
            _basketRepository = basketRepository;
            _discountGrpcService = discountGrpcService;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet("{userName}",Name ="GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string username)
        {
            var basket = await _basketRepository.GetBasket(username);
            return Ok(basket ?? new ShoppingCart(username));
        }
        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            //TODO :Comnucate WIth DISCOUNT MICROSERVICE
            foreach (var item in basket.Items)
            {
               var coupon=await _discountGrpcService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }

            return Ok(await _basketRepository.UpdateBasket(basket));
        }
        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _basketRepository.DeleteBasket(userName);
            return Ok();
        }

        [HttpPost("checkout")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout checkout)
        {
            var basket = await _basketRepository.GetBasket(checkout.UserName);
            if (basket is null)
                return BadRequest("Basket not found.");

            var eventMessage = new BasketCheckoutIntegrationEvent
            {
                UserName = checkout.UserName,
                TotalPrice = basket.TotalPrice,
                FirstName = checkout.FirstName,
                LastName = checkout.LastName,
                EmailAddress = checkout.EmailAddress,
                AddressLine = checkout.AddressLine,
                Country = checkout.Country,
                State = checkout.State,
                ZipCode = checkout.ZipCode,
                CardName = checkout.CardName,
                CardNumber = checkout.CardNumber,
                Expiration = checkout.Expiration,
                CVV = checkout.CVV,
                PaymentMethod = checkout.PaymentMethod
            };

            await _publishEndpoint.Publish(eventMessage);
            await _basketRepository.DeleteBasket(checkout.UserName);

            return Accepted();
        }
    }
}
