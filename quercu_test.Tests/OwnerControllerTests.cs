using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quercu_test.Server.Controllers;
using quercu_test.Server.Data;
using quercu_test.Server.Models;
using Xunit;

namespace quercu_test.Tests
{
    public class OwnerControllerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly OwnerController _controller;

        public OwnerControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new OwnerController(_context);
            SeedTestData();
        }

        private void SeedTestData()
        {
            _context.Owners.AddRange(
                new Owner
                {
                    Id = 1,
                    Name = "Juan Pérez",
                    Telephone = "555-1234",
                    Email = "juan@example.com",
                    IdentificationNumber = "123456789",
                    Address = "Calle Principal 123"
                },
                new Owner
                {
                    Id = 2,
                    Name = "María García",
                    Telephone = "555-5678",
                    Email = "maria@example.com",
                    IdentificationNumber = "987654321",
                    Address = "Avenida Central 456"
                }
            );
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetOwners_ReturnsAllItems()
        {
            var result = await _controller.GetOwners();

            var actionResult = Assert.IsType<ActionResult<IEnumerable<Owner>>>(result);
            var returnValue = Assert.IsType<List<Owner>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetOwner_ExistingId_ReturnsOwner()
        {
            var result = await _controller.GetOwner(1);

            var actionResult = Assert.IsType<ActionResult<Owner>>(result);
            var owner = Assert.IsType<Owner>(actionResult.Value);
            Assert.Equal("Juan Pérez", owner.Name);
        }

        [Fact]
        public async Task GetOwner_NonExistingId_ReturnsNotFound()
        {
            var result = await _controller.GetOwner(99);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostOwner_ValidData_ReturnsCreated()
        {
            var newOwner = new Owner
            {
                Name = "Carlos López",
                Telephone = "555-9012",
                Email = "carlos@example.com",
                IdentificationNumber = "1122334455",
                Address = "Boulevard Norte 789"
            };

            var result = await _controller.PostOwner(newOwner);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdItem = Assert.IsType<Owner>(createdAtActionResult.Value);
            Assert.Equal("Carlos López", createdItem.Name);
            Assert.Equal(3, await _context.Owners.CountAsync());
        }

        [Fact]
        public async Task PostOwner_DuplicateIdentification_ReturnsBadRequest()
        {
            var newOwner = new Owner
            {
                Name = "Pedro Duplicado",
                Telephone = "555-0000",
                IdentificationNumber = "123456789", 
                Email = "pedro@example.com",
                Address = "Calle Duplicada 123"
            };

            var result = await _controller.PostOwner(newOwner);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("El número de identificación ya está registrado",
                badRequestResult.Value.GetType().GetProperty("message")?.GetValue(badRequestResult.Value));
        }

        [Fact]
        public async Task PutOwner_ValidUpdate_ReturnsNoContent()
        {
          
            var originalOwner = await _context.Owners.FindAsync(1);
            _context.Entry(originalOwner).State = EntityState.Detached;

            var updatedOwner = new Owner
            {
                Id = 1,
                Name = "Juan Pérez Modificado",
                Telephone = "555-4321",
                IdentificationNumber = "123456789", 
                Email = "nuevoemail@example.com"
            };
          
            var result = await _controller.PutOwner(1, updatedOwner);

            Assert.IsType<NoContentResult>(result);
            var storedItem = await _context.Owners.AsNoTracking().FirstAsync(o => o.Id == 1);
            Assert.Equal("Juan Pérez Modificado", storedItem.Name);
        }

        [Fact]
        public async Task PutOwner_DuplicateIdentification_ReturnsBadRequest()
        {
            var updatedOwner = new Owner
            {
                Id = 1,
                Name = "Juan Pérez",
                Telephone = "555-1234",
                IdentificationNumber = "987654321" // Pertenece al owner 2
            };

            var result = await _controller.PutOwner(1, updatedOwner);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteOwner_NoProperties_DeletesSuccessfully()
        {
            var result = await _controller.DeleteOwner(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.Owners.FindAsync(1));
        }

        [Fact]
        public async Task DeleteOwner_WithProperties_ReturnsConflict()
        {
            // Agregar propiedad asociada
            _context.Properties.Add(new Property
            {
                Number = "P-001",
                Address = "Calle Test 123",
                Area = 100,
                OwnerId = 1,
                PropertyTypeId = 1
            });
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteOwner(1);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            var message = conflictResult.Value.GetType().GetProperty("message")?.GetValue(conflictResult.Value);
            Assert.Contains("tiene propiedades registradas", message.ToString());
        }
    }
}