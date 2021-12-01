using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Catalog.API.Repository;
using Microsoft.Extensions.Logging;
using Catalog.API.Entities;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver.Core.Authentication;

namespace Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _repo;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository repo, ILogger<CatalogController> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _repo.GetProducts();

            return Ok(products);
        }

        [HttpGet("{productId:length(24)}", Name = nameof(GetProductById))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProductById(string productId)
        {
            var product = await _repo.GetProductById(productId);

            if (product is not null) return Ok(product);

            _logger.LogError($"product with id {productId} not found");
            return NotFound();

        }

        [HttpGet("[action]/{productCategory}", Name = nameof(GetProductByCategory))]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string productCategory)
        {
            var products = await _repo.GetProductsByCategory(productCategory);

            return Ok(products);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> PostProduct([FromBody] Product product)
        {
            await _repo.CreateProduct(product);

            return CreatedAtRoute(nameof(GetProductById), new {productId = product.Id}, product);
        }

        [HttpPut("{productId:length(24)}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateRepository([FromRoute] string productId, [FromBody] Product product)
        {
            var productFromDb = await _repo.GetProductById(productId);

            if (productFromDb is null)
            {
                _logger.LogError($"product with id {productId} not found");
                return NotFound();
            }

            if (productFromDb.Id != productId)
            {
                _logger.LogError($"request denied for mismatch in product ids {productId} and {product.Id}");
                return BadRequest();
            }

            await _repo.UpdateProduct(product);

            return NoContent();

        }

        [HttpDelete("{productId:length(24)}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteProduct(string productId)
        {
            var product = await _repo.GetProductById(productId);

            if(product is null)
            {
                _logger.LogError($"product with id {productId} not found");
                return NotFound();
            }

            await _repo.DeleteProduct(productId);

            return NoContent();
        }
    }
}