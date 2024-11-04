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
    public class AddressTypeController : ControllerBase
    {
        private readonly WebRestOracleContext _context;

        public AddressTypeController(WebRestOracleContext context)
        {
            _context = context;
        }

        // GET: api/AddressType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressType>>> GetAddressTypes()
        {
            return await _context.AddressTypes.ToListAsync();
        }

        // GET: api/AddressType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AddressType>> GetAddressType(string id)
        {
            var addressType = await _context.AddressTypes.FindAsync(id);

            if (addressType == null)
            {
                return NotFound();
            }

            return addressType;
        }

        // PUT: api/AddressType/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddressType(string id, AddressType addressType)
        {
            if (id != addressType.AddressTypeId)
            {
                return BadRequest();
            }

            _context.Entry(addressType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressTypeExists(id))
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
                // Handle database update exceptions (e.g., constraint violations)
                return BadRequest(new { message = ex.Message });
            }

            return NoContent();
        }

        // POST: api/AddressType
        [HttpPost]
        public async Task<ActionResult<AddressType>> PostAddressType(AddressType addressType)
        {
            _context.AddressTypes.Add(addressType);
            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetAddressType), new { id = addressType.AddressTypeId }, addressType);
            }
            catch (DbUpdateException ex)
            {
                // Handle database update exceptions (e.g., constraint violations)
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/AddressType/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddressType(string id)
        {
            var addressType = await _context.AddressTypes.FindAsync(id);
            if (addressType == null)
            {
                return NotFound();
            }

            _context.AddressTypes.Remove(addressType);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Handle database update exceptions (e.g., constraint violations)
                return BadRequest(new { message = ex.Message });
            }

            return NoContent();
        }

        private bool AddressTypeExists(string id)
        {
            return _context.AddressTypes.Any(e => e.AddressTypeId == id);
        }
    }
}
