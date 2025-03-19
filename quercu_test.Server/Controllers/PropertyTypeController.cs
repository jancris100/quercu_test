using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quercu_test.Server.Data;
using quercu_test.Server.Models;

namespace quercu_test.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyTypeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PropertyTypeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PropertyType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyType>>> GetPropertyTypes()
        {
            return await _context.PropertyTypes.ToListAsync();
        }

        // GET: api/PropertyType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyType>> GetPropertyType(int id)
        {
            var propertyType = await _context.PropertyTypes.FindAsync(id);

            if (propertyType == null)
            {
                return NotFound();
            }

            return propertyType;
        }

        // POST: api/PropertyType
        [HttpPost]
        public async Task<ActionResult<PropertyType>> PostPropertyType(PropertyType propertyType)
        {
            _context.PropertyTypes.Add(propertyType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPropertyType), new { id = propertyType.Id }, propertyType);
        }

        // PUT: api/PropertyType/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPropertyType(int id, PropertyType propertyType)
        {
            if (id != propertyType.Id)
            {
                return BadRequest();
            }

            _context.Entry(propertyType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropertyTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/PropertyType/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePropertyType(int id)
        {
            var propertyType = await _context.PropertyTypes.FindAsync(id);
            if (propertyType == null)
            {
                return NotFound();
            }

            _context.PropertyTypes.Remove(propertyType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PropertyTypeExists(int id)
        {
            return _context.PropertyTypes.Any(e => e.Id == id);
        }
    }
}
