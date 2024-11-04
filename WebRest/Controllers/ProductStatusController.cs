using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebRestEF.EF.Data;
using WebRestEF.EF.Models;

namespace WebRestEF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductStatusController : ControllerBase
    {
        private readonly WebRestOracleContext _context;

        public ProductStatusController(WebRestOracleContext context)
        {
            _context = context;
        }

        // GET: api/ProductStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductStatus>>> GetProductStatuses()
        {
            return await _context.ProductStatuses
                .Include(ps => ps.Products)
                .ToListAsync();
        }

        // GET: api/ProductStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductStatus>> GetProductStatus(string id)
        {
            var productStatus = await _context.ProductStatuses
                .Include(ps => ps.Products)
                .FirstOrDefaultAsync(ps => ps.ProductStatusId == id);

            if (productStatus == null)
            {
                return NotFound();
            }

            return productStatus;
        }

        // PUT: api/ProductStatus/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductStatus(string id, ProductStatus productStatus)
        {
            if (id != productStatus.ProductStatusId)
            {
                return BadRequest();
            }

            productStatus.ProductStatusUpdtDt = DateTime.Now;

            _context.Entry(productStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductStatusExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return NoContent();
        }

        // POST: api/ProductStatus
        [HttpPost]
        public async Task<ActionResult<ProductStatus>> PostProductStatus(ProductStatus productStatus)
        {
            productStatus.ProductStatusCrtdDt = DateTime.Now;
            productStatus.ProductStatusUpdtDt = DateTime.Now;

            _context.ProductStatuses.Add(productStatus);

            try
            {
                await _context.SaveChangesAsync();

                await _context.Entry(productStatus).Collection(ps => ps.Products).LoadAsync();

                return CreatedAtAction(nameof(GetProductStatus), new { id = productStatus.ProductStatusId }, productStatus);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/ProductStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductStatus(string id)
        {
            var productStatus = await _context.ProductStatuses
                .Include(ps => ps.Products)
                .FirstOrDefaultAsync(ps => ps.ProductStatusId == id);

            if (productStatus == null)
            {
                return NotFound();
            }

            if (productStatus.Products.Any())
            {
                return BadRequest(new { message = "Cannot delete product status with related products." });
            }

            _context.ProductStatuses.Remove(productStatus);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return NoContent();
        }

        private bool ProductStatusExists(string id)
        {
            return _context.ProductStatuses.Any(e => e.ProductStatusId == id);
        }
    }
}
