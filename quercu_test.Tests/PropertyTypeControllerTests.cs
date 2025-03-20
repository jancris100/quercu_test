using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quercu_test.Server.Controllers;
using quercu_test.Server.Data;
using quercu_test.Server.Models;
using Xunit;

namespace quercu_test.Tests
{
    public class PropertyTypeControllerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly PropertyTypeController _controller;

        public PropertyTypeControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            _context = new AppDbContext(options);
            _controller = new PropertyTypeController(_context);

            SeedTestData();
        }

        private void SeedTestData()
        {
            _context.PropertyTypes.AddRange(
                new PropertyType { Id = 1, Description = "Casa" },
                new PropertyType { Id = 2, Description = "Departamento" }
            );
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetPropertyTypes_ReturnsAllItems()
        {
            var result = await _controller.GetPropertyTypes();

            var actionResult = Assert.IsType<ActionResult<IEnumerable<PropertyType>>>(result);
            var returnValue = Assert.IsType<List<PropertyType>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task GetPropertyType_ExistingId_ReturnsItem(int id)
        {
            var result = await _controller.GetPropertyType(id);

            var actionResult = Assert.IsType<ActionResult<PropertyType>>(result);
            var propertyType = Assert.IsType<PropertyType>(actionResult.Value);
            Assert.Equal(id, propertyType.Id);
        }

        [Fact]
        public async Task GetPropertyType_NonExistingId_ReturnsNotFound()
        {
            var nonExistingId = 99;

            var result = await _controller.GetPropertyType(nonExistingId);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostPropertyType_ValidData_ReturnsCreated()
        {
            var newType = new PropertyType { Description = "Oficina" };

            var result = await _controller.PostPropertyType(newType);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdItem = Assert.IsType<PropertyType>(createdAtActionResult.Value);
            Assert.Equal("Oficina", createdItem.Description);
            Assert.Equal(3, _context.PropertyTypes.Count());
        }

        [Fact]
        public async Task PutPropertyType_ValidUpdate_ReturnsNoContent()
        {
            var originalType = await _context.PropertyTypes.FindAsync(1);

            _context.Entry(originalType).State = EntityState.Detached;

            var updatedType = new PropertyType
            {
                Id = 1,
                Description = "Casa Modificada"
            };

            var result = await _controller.PutPropertyType(1, updatedType);

            Assert.IsType<NoContentResult>(result);

            var storedItem = await _context.PropertyTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == 1);

            Assert.Equal("Casa Modificada", storedItem.Description);
        }

        [Fact]
        public async Task PutPropertyType_IdMismatch_ReturnsBadRequest()
        {
            var updatedType = new PropertyType { Id = 1, Description = "Casa Modificada" };

            var result = await _controller.PutPropertyType(2, updatedType);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeletePropertyType_NotInUse_DeletesSuccessfully()
        {
            var result = await _controller.DeletePropertyType(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.PropertyTypes.FindAsync(1));
            Assert.Single(_context.PropertyTypes);
        }

        [Fact]
        public async Task DeletePropertyType_InUse_ReturnsConflict()
        {
            var owner = new Owner { Id = 1, Name = "Propietario Test", Email = "test@example.com" };
            var propertyType = await _context.PropertyTypes.FindAsync(1); // Obtener el tipo "Casa"

            _context.Properties.Add(new Property
            {
                Number = "P-001",
                Address = "Calle Principal 123",
                Area = 150.75m,
                ConstructionArea = 120.50m,
                PropertyTypeId = propertyType.Id, 
                OwnerId = owner.Id 
            });

            await _context.SaveChangesAsync();

            var result = await _controller.DeletePropertyType(propertyType.Id);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result);

            var message = conflictResult.Value.GetType().GetProperty("message")?.GetValue(conflictResult.Value);
            Assert.Equal(
                $"No se puede eliminar '{propertyType.Description}' porque está siendo usado en propiedades",
                message
            );

            Assert.NotNull(await _context.PropertyTypes.FindAsync(propertyType.Id));
        }
    }
}