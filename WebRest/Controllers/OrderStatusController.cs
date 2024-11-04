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
    public class OrderStatusController : ControllerBase
    {
        private readonly WebRestOracleContext _context;

        public OrderStatusController(WebRestOracleContext context)
        {
            _context = context;
        }

        // GET: api/OrderStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderStatus>>> GetOrderStatuses()
        {
            return await _context.OrderStatuses
                .Include(os => os.OrderStatusNextOrderStatus)
                .Include(os => os.InverseOrderStatusNextOrderStatus)
                .ToListAsync();
        }

        // GET: api/OrderStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderStatus>> GetOrderStatus(string id)
        {
            var orderStatus = await _context.OrderStatuses
                .Include(os => os.OrderStatusNextOrderStatus)
                .Include(os => os.InverseOrderStatusNextOrderStatus)
                .FirstOrDefaultAsync(os => os.OrderStatusId == id);

            if (orderStatus == null)
            {
                return NotFound();
            }

            return orderStatus;
        }

        // PUT: api/OrderStatus/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderStatus(string id, OrderStatus orderStatus)
        {
            if (id != orderStatus.OrderStatusId)
            {
                return BadRequest();
            }

            orderStatus.OrderStatusUpdtDt = DateTime.Now;

            _context.Entry(orderStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderStatusExists(id))
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

        // POST: api/OrderStatus
        [HttpPost]
        public async Task<ActionResult<OrderStatus>> PostOrderStatus(OrderStatus orderStatus)
        {
            orderStatus.OrderStatusCrtdDt = DateTime.Now;
            orderStatus.OrderStatusUpdtDt = DateTime.Now;

            _context.OrderStatuses.Add(orderStatus);

            try
            {
                await _context.SaveChangesAsync();

                await _context.Entry(orderStatus).Reference(os => os.OrderStatusNextOrderStatus).LoadAsync();
                await _context.Entry(orderStatus).Collection(os => os.InverseOrderStatusNextOrderStatus).LoadAsync();

                return CreatedAtAction(nameof(GetOrderStatus), new { id = orderStatus.OrderStatusId }, orderStatus);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/OrderStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderStatus(string id)
        {
            var orderStatus = await _context.OrderStatuses
                .Include(os => os.InverseOrderStatusNextOrderStatus)
                .Include(os => os.OrderStates)
                .FirstOrDefaultAsync(os => os.OrderStatusId == id);

            if (orderStatus == null)
            {
                return NotFound();
            }

            if (orderStatus.InverseOrderStatusNextOrderStatus.Any() || orderStatus.OrderStates.Any())
            {
                return BadRequest(new { message = "Cannot delete OrderStatus with related records." });
            }

            _context.OrderStatuses.Remove(orderStatus);

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

        private bool OrderStatusExists(string id)
        {
            return _context.OrderStatuses.Any(e => e.OrderStatusId == id);
        }
    }
}
