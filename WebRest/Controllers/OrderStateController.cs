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
    public class OrderStateController : ControllerBase
    {
        private readonly WebRestOracleContext _context;

        public OrderStateController(WebRestOracleContext context)
        {
            _context = context;
        }

        // GET: api/OrderState
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderState>>> GetOrderStates()
        {
            return await _context.OrderStates
                .Include(os => os.OrderStateOrders)
                .Include(os => os.OrderStateOrderStatus)
                .ToListAsync();
        }

        // GET: api/OrderState/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderState>> GetOrderState(string id)
        {
            var orderState = await _context.OrderStates
                .Include(os => os.OrderStateOrders)
                .Include(os => os.OrderStateOrderStatus)
                .FirstOrDefaultAsync(os => os.OrderStateId == id);

            if (orderState == null)
            {
                return NotFound();
            }

            return orderState;
        }

        // PUT: api/OrderState/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderState(string id, OrderState orderState)
        {
            if (id != orderState.OrderStateId)
            {
                return BadRequest();
            }

            orderState.OrderStateUpdtDt = DateTime.Now;

            _context.Entry(orderState).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderStateExists(id))
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

        // POST: api/OrderState
        [HttpPost]
        public async Task<ActionResult<OrderState>> PostOrderState(OrderState orderState)
        {
            orderState.OrderStateCrtdDt = DateTime.Now;
            orderState.OrderStateUpdtDt = DateTime.Now;

            _context.OrderStates.Add(orderState);

            try
            {
                await _context.SaveChangesAsync();

                await _context.Entry(orderState).Reference(os => os.OrderStateOrders).LoadAsync();
                await _context.Entry(orderState).Reference(os => os.OrderStateOrderStatus).LoadAsync();

                return CreatedAtAction(nameof(GetOrderState), new { id = orderState.OrderStateId }, orderState);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/OrderState/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderState(string id)
        {
            var orderState = await _context.OrderStates.FindAsync(id);
            if (orderState == null)
            {
                return NotFound();
            }

            _context.OrderStates.Remove(orderState);

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

        private bool OrderStateExists(string id)
        {
            return _context.OrderStates.Any(e => e.OrderStateId == id);
        }
    }
}
