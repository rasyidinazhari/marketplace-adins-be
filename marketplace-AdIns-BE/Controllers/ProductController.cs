using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Services;
using System.ComponentModel.DataAnnotations;
using System.Data;
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
            [FromQuery] int limit = 10,
            [FromQuery] long categoryId = 0
            )
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            long? userId = null;
            if (long.TryParse(userIdClaim, out long parsedId))
            {
                userId = parsedId;
            }
            string? role = User.FindFirst(ClaimTypes.Role)?.Value;
            List<Product> products = new List<Product>();

            // Get All For Buyer
            if (userIdClaim == null || role == "Buyer")
            {
                products = await _productService.GetAllAsync(search, null, categoryId, page, limit);
            } else // Get All For Seller
            {
                products = await _productService.GetAllAsync(search, userId, categoryId, page, limit);
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

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequest request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId) || userId <= 0)
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            try
            {
                var result = await _productService.CreateAsync(userId, request);
                if (result == null) return NotFound();

                return Created($"/api/products/{result.Id}", result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
        }

        [Authorize(Roles = "Seller")]
        [HttpPut("{productId:long}")]
        public async Task<IActionResult> UpdateProduct(long productId, [FromBody] ProductRequest request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId) || userId <= 0)
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            try
            {
                var result = await _productService.UpdateAsync(productId, userId, request);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DBConcurrencyException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Seller")]
        [HttpDelete("{productId:long}")]
        public async Task<IActionResult> DeleteProduct(long productId)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId) || userId <= 0)
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            var success = await _productService.DeleteAsync(productId, userId);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
