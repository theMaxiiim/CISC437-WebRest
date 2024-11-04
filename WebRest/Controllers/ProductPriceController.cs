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
    public class ProductPriceController : ControllerBase
    {
        private readonly WebRestOracleContext _context;

        public ProductPriceController(WebRestOracleContext context)
        {
            _context = context;
        }

        // GET: api/ProductPrice
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductPrice>>> GetProductPrices()
        {
            return await _context.ProductPrices
                .Include(pp => pp.ProductPriceProduct)
                .ToListAsync();
        }

        // GET: api/ProductPrice/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductPrice>> GetProductPrice(string id)
        {
            var productPrice = await _context.ProductPrices
                .Include(pp => pp.ProductPriceProduct)
                .FirstOrDefaultAsync(pp => pp.ProductPriceId == id);

            if (productPrice == null)
            {
                return NotFound();
            }

            return productPrice;
        }

        // PUT: api/ProductPrice/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductPrice(string id, ProductPrice productPrice)
        {
            if (id != productPrice.ProductPriceId)
            {
                return BadRequest();
            }

            productPrice.ProductPriceUpdtDt = DateTime.Now;

            _context.Entry(productPrice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductPriceExists(id))
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

        // POST: api/ProductPrice
        [HttpPost]
        public async Task<ActionResult<ProductPrice>> PostProductPrice(ProductPrice productPrice)
        {
            productPrice.ProductPriceCrtdDt = DateTime.Now;
            productPrice.ProductPriceUpdtDt = DateTime.Now;

            _context.ProductPrices.Add(productPrice);

            try
            {
                await _context.SaveChangesAsync();

                await _context.Entry(productPrice).Reference(pp => pp.ProductPriceProduct).LoadAsync();

                return CreatedAtAction(nameof(GetProductPrice), new { id = productPrice.ProductPriceId }, productPrice);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/ProductPrice/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductPrice(string id)
        {
            var productPrice = await _context.ProductPrices.FindAsync(id);
            if (productPrice == null)
            {
                return NotFound();
            }

            _context.ProductPrices.Remove(productPrice);

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

        private bool ProductPriceExists(string id)
        {
            return _context.ProductPrices.Any(e => e.ProductPriceId == id);
        }
    }
}
