using Basket.API.Entities;
using Basket.API.GRPCService;
using Basket.API.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Http;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repo;
        private readonly DiscountGrpcService _discountService;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public BasketController(IBasketRepository repo,
            DiscountGrpcService discountService, IMapper mapper,
            IPublishEndpoint publishEndpoint)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _discountService = discountService ?? throw new ArgumentNullException(nameof(discountService));
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet("{userName}", Name = nameof(GetBasket))]
        [ProducesResponseType(typeof(ShoppingCart), (StatusCodes.Status200OK))]
        public async Task<ActionResult<ShoppingCart>> GetBasket([FromRoute]string userName)
        {
            var basket = await _repo.GetBasket(userName);

            return Ok(basket ?? new ShoppingCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), StatusCodes.Status204NoContent)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            foreach(var item in basket.Items)
               item.Price -= (await _discountService.GetDiscount(item.ProductName)).Amount;
            return Ok(await _repo.UpdateBasket(basket));
        }


        [HttpDelete("{userName}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteBasket([FromRoute]string userName)
        {
            await _repo.DeleteBasket(userName);

            return NoContent();
        }

        [HttpPost("[action]", Name = nameof(Checkout))]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            var basket = await _repo.GetBasket(basketCheckout.UserName);

            if (basket is null) return BadRequest();

            var basketCheckoutEvent = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            basketCheckoutEvent.TotalPrice = basket.TotalPrice;

            await _publishEndpoint.Publish(basketCheckoutEvent);

            await _repo.DeleteBasket(basket.UserName);
            return Accepted();
        }
    }
}
