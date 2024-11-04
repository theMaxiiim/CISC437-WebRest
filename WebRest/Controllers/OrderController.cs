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
    public class OrderController : ControllerBase
    {
        private readonly WebRestOracleContext _context;

        public OrderController(WebRestOracleContext context)
        {
            _context = context;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.OrdersCustomer)
                .Include(o => o.OrderStates)
                .Include(o => o.OrdersLines)
                .ToListAsync();
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(string id)
        {
            var order = await _context.Orders
                .Include(o => o.OrdersCustomer)
                .Include(o => o.OrderStates)
                .Include(o => o.OrdersLines)
                .FirstOrDefaultAsync(o => o.OrdersId == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Order/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(string id, Order order)
        {
            if (id != order.OrdersId)
            {
                return BadRequest();
            }

            order.OrdersUpdtDt = DateTime.Now;

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/Order
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            order.OrdersCrtdDt = DateTime.Now;
            order.OrdersUpdtDt = DateTime.Now;

            _context.Orders.Add(order);

            try
            {
                await _context.SaveChangesAsync();

                await _context.Entry(order).Reference(o => o.OrdersCustomer).LoadAsync();
                await _context.Entry(order).Collection(o => o.OrderStates).LoadAsync();
                await _context.Entry(order).Collection(o => o.OrdersLines).LoadAsync();

                return CreatedAtAction(nameof(GetOrder), new { id = order.OrdersId }, order);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderStates)
                .Include(o => o.OrdersLines)
                .FirstOrDefaultAsync(o => o.OrdersId == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.OrderStates.Any() || order.OrdersLines.Any())
            {
                return BadRequest(new { message = "Cannot delete order with related order states or order lines." });
            }

            _context.Orders.Remove(order);

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

        private bool OrderExists(string id)
        {
            return _context.Orders.Any(e => e.OrdersId == id);
        }
    }
}
