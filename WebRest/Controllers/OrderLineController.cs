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
    public class OrdersLineController : ControllerBase
    {
        private readonly WebRestOracleContext _context;

        public OrdersLineController(WebRestOracleContext context)
        {
            _context = context;
        }

        // GET: api/OrdersLine
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdersLine>>> GetOrdersLines()
        {
            return await _context.OrdersLines
                .Include(ol => ol.OrdersLineOrders)
                .Include(ol => ol.OrdersLineProduct)
                .ToListAsync();
        }

        // GET: api/OrdersLine/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdersLine>> GetOrdersLine(string id)
        {
            var ordersLine = await _context.OrdersLines
                .Include(ol => ol.OrdersLineOrders)
                .Include(ol => ol.OrdersLineProduct)
                .FirstOrDefaultAsync(ol => ol.OrdersLineId == id);

            if (ordersLine == null)
            {
                return NotFound();
            }

            return ordersLine;
        }

        // PUT: api/OrdersLine/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrdersLine(string id, OrdersLine ordersLine)
        {
            if (id != ordersLine.OrdersLineId)
            {
                return BadRequest();
            }

            ordersLine.OrdersLineUpdtDt = DateTime.Now;

            _context.Entry(ordersLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdersLineExists(id))
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

        // POST: api/OrdersLine
        [HttpPost]
        public async Task<ActionResult<OrdersLine>> PostOrdersLine(OrdersLine ordersLine)
        {
            ordersLine.OrdersLineCrtdDt = DateTime.Now;
            ordersLine.OrdersLineUpdtDt = DateTime.Now;

            _context.OrdersLines.Add(ordersLine);
            try
            {
                await _context.SaveChangesAsync();

                await _context.Entry(ordersLine).Reference(ol => ol.OrdersLineOrders).LoadAsync();
                await _context.Entry(ordersLine).Reference(ol => ol.OrdersLineProduct).LoadAsync();

                return CreatedAtAction(nameof(GetOrdersLine), new { id = ordersLine.OrdersLineId }, ordersLine);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/OrdersLine/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrdersLine(string id)
        {
            var ordersLine = await _context.OrdersLines.FindAsync(id);
            if (ordersLine == null)
            {
                return NotFound();
            }

            _context.OrdersLines.Remove(ordersLine);

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

        private bool OrdersLineExists(string id)
        {
            return _context.OrdersLines.Any(e => e.OrdersLineId == id);
        }
    }
}
