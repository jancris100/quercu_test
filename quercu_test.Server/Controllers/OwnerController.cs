using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quercu_test.Server.Data;
using quercu_test.Server.Models;

namespace quercu_test.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OwnerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Owner
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Owner>>> GetOwners()
        {
            return await _context.Owners.ToListAsync();
        }

        // GET: api/Owner/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Owner>> GetOwner(int id)
        {
            var owner = await _context.Owners.FindAsync(id);

            if (owner == null)
            {
                return NotFound();
            }

            return owner;
        }

        // POST: api/Owner
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Owner>> PostOwner(Owner owner)
        {

            if (await _context.Owners.AnyAsync(o => o.IdentificationNumber == owner.IdentificationNumber))
            {
                return BadRequest(new { message = "El número de identificación ya está registrado" });
            }

            _context.Owners.Add(owner);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOwner), new { id = owner.Id }, owner);
        }

        // PUT: api/Owner/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOwner(int id, Owner owner)
        {
            if (id != owner.Id)
            {
                return BadRequest();
            }

            var existing = await _context.Owners
            .FirstOrDefaultAsync(o => o.IdentificationNumber == owner.IdentificationNumber && o.Id != id);

            if (existing != null)
            {
                return BadRequest(new { message = "El número de identificación ya está registrado" });
            }

            _context.Entry(owner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OwnerExists(id))
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

        // DELETE: api/Owner/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOwner(int id)
        {
            var owner = await _context.Owners.FindAsync(id);
            if (owner == null)
            {
                return NotFound();
            }

            bool hasProperties = await _context.Properties.AnyAsync(p => p.OwnerId == id);
            if (hasProperties)
            {
                return Conflict(new
                {
                    message = $"No se puede eliminar a {owner.Name} porque tiene propiedades registradas"
                });
            }

            _context.Owners.Remove(owner);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OwnerExists(int id)
        {
            return _context.Owners.Any(e => e.Id == id);
        }
    }
}
