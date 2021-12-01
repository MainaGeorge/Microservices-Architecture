using Discount.API.Entities;
using Discount.API.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Discount.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepository _repo;
        public DiscountController(IDiscountRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("{productName}", Name = nameof(GetDiscount))]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Coupon>> GetDiscount(string productName)
        {
            var coupon = await _repo.GetDiscount(productName);
            return Ok(coupon);
        }


        [HttpPost]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.Created)]

        public async Task<ActionResult<Coupon>> CreateDiscount([FromBody] Coupon coupon)
        {
            await _repo.CreateDiscount(coupon);

            return CreatedAtRoute(nameof(GetDiscount), new { coupon.ProductName }, coupon);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Coupon>> UpdateCoupon([FromBody] Coupon coupon)
        {
            return Ok(await _repo.UpdateDiscount(coupon));
        }

        [HttpDelete("{productName}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteDiscount(string productName)
        {
            await _repo.DeleteDiscount(productName);

            return NoContent();
        }
    
    }
}
