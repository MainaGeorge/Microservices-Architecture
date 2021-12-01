using Basket.API.Entites;
using Basket.API.GRPCService;
using Basket.API.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repo;
        private readonly DiscountGrpcService _discountService;

        public BasketController(IBasketRepository repo, DiscountGrpcService discountService)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _discountService = discountService ?? throw new ArgumentNullException(nameof(discountService));
        }

        [HttpGet("{userName}", Name = nameof(GetBasket))]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await _repo.GetBasket(userName);

            return Ok(basket ?? new ShoppingCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            foreach(var item in basket.Items)
               item.Price -= (await _discountService.GetDiscount(item.ProductName)).Amount;
            return Ok(await _repo.UpdateBasket(basket));
        }


        [HttpDelete("{userName}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _repo.DeleteBasket(userName);

            return NoContent();
        }
    }
}
