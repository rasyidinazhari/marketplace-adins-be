using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Services;
using System.Security.Claims;
using marketplace_AdIns_BE.Models;

namespace marketplace_AdIns_BE.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {
            var userIdClaim = User.FindFirst("userId`")?.Value;
            string? role = User.FindFirst(ClaimTypes.Role)?.Value;
            List<Product> products = new List<Product>();

            // Get All For Buyer
            if (userIdClaim == null || role == "Buyer")
            {
                products = await _productService.GetAllAsync(search, null, page, limit);
            } else // Get All For Seller
            {
                products = await _productService.GetAllAsync(search, null, page, limit);
            }
            return Ok(products);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetProduct(long id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequest request)
        {
            try
            {
                var result = await _productService.CreateAsync(request);
                return Created($"/api/products/{result.Id}", result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{productId:long}")]
        public async Task<IActionResult> UpdateProduct(long productId, [FromBody] ProductRequest request)
        {
            var result = await _productService.UpdateAsync(productId, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{productId:long}")]
        public async Task<IActionResult> DeleteProduct(long productId)
        {
            var success = await _productService.DeleteAsync(productId);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
