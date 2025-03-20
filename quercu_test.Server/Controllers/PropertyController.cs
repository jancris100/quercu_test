using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quercu_test.Server.Data;
using quercu_test.Server.DTOS;
using quercu_test.Server.Models;

namespace quercu_test.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PropertyController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Property
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Property>>> GetProperties()
        {
            return await _context.Properties.Include(p => p.Owner).Include(p => p.PropertyType).ToListAsync();
        }

        // GET: api/Property/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Property>> GetProperty(int id)
        {
            var property = await _context.Properties.Include(p => p.Owner).Include(p => p.PropertyType)
                                                     .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
            {
                return NotFound();
            }

            return property;
        }

        // POST: api/Property
        [HttpPost]
        public async Task<ActionResult<Property>> PostProperty([FromBody] PropertyCreateDto dto)
        {
            var property = new Property
            {
                PropertyTypeId = dto.PropertyTypeId,
                OwnerId = dto.OwnerId,
                Number = dto.Number,
                Address = dto.Address,
                Area = dto.Area,
                ConstructionArea = dto.ConstructionArea
            };

            if (!await _context.Owners.AnyAsync(o => o.Id == dto.OwnerId))
            {
                return BadRequest("El dueño especificado no existe");
            }

            if (!await _context.PropertyTypes.AnyAsync(t => t.Id == dto.PropertyTypeId))
            {
                return BadRequest("El tipo de propiedad especificado no existe");
            }

            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProperty", new { id = property.Id }, property);
        }

        // PUT: api/Property/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProperty(int id, [FromBody] PropertyUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            if (!await _context.PropertyTypes.AnyAsync(pt => pt.Id == dto.PropertyTypeId))
            {
                return BadRequest("Tipo de propiedad no válido");
            }

            if (!await _context.Owners.AnyAsync(o => o.Id == dto.OwnerId))
            {
                return BadRequest("Dueño no válido");
            }

            property.PropertyTypeId = dto.PropertyTypeId;
            property.OwnerId = dto.OwnerId;
            property.Number = dto.Number;
            property.Address = dto.Address;
            property.Area = dto.Area;
            property.ConstructionArea = dto.ConstructionArea;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropertyExistsInDatabase(id))
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

        private bool PropertyExistsInDatabase(int id)
        {
            return _context.Properties.Any(e => e.Id == id);
        }


        // DELETE: api/Property/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.Id == id);
        }
    }
}
