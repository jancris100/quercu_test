using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quercu_test.Server.Controllers;
using quercu_test.Server.Data;
using quercu_test.Server.DTOS;
using quercu_test.Server.Models;
using Xunit;

namespace quercu_test.Tests
{
    public class PropertyControllerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly PropertyController _controller;

        public PropertyControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new PropertyController(_context);
            SeedTestData();
        }

        private void SeedTestData()
        {
            // Propietarios
            _context.Owners.AddRange(
                new Owner { Id = 1, Name = "Dueño 1", Telephone = "11111111", IdentificationNumber = "1111" },
                new Owner { Id = 2, Name = "Dueño 2", Telephone = "22222222", IdentificationNumber = "2222" }
            );

            // Tipos de propiedad
            _context.PropertyTypes.AddRange(
                new PropertyType { Id = 1, Description = "Casa" },
                new PropertyType { Id = 2, Description = "Departamento" }
            );

            // Propiedades
            _context.Properties.Add(new Property
            {
                Id = 1,
                PropertyTypeId = 1,
                OwnerId = 1,
                Number = "P-001",
                Address = "Calle Existente 123",
                Area = 100,
                ConstructionArea = 80
            });

            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetProperties_ReturnsAllPropertiesWithRelations()
        {
            // Act
            var result = await _controller.GetProperties();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Property>>>(result);
            var properties = Assert.IsType<List<Property>>(actionResult.Value);
            Assert.Single(properties);
            Assert.NotNull(properties[0].Owner);
            Assert.NotNull(properties[0].PropertyType);
        }

        [Fact]
        public async Task GetProperty_ExistingId_ReturnsPropertyWithRelations()
        {
            // Act
            var result = await _controller.GetProperty(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Property>>(result);
            var property = actionResult.Value;
            Assert.Equal(1, property.Id);
            Assert.Equal("Dueño 1", property.Owner.Name);
            Assert.Equal("Casa", property.PropertyType.Description);
        }

        [Fact]
        public async Task GetProperty_NonExistingId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetProperty(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostProperty_ValidData_CreatesNewProperty()
        {
            // Arrange
            var dto = new PropertyCreateDto
            {
                PropertyTypeId = 2,
                OwnerId = 2,
                Number = "P-002",
                Address = "Nueva Dirección 456",
                Area = 150,
                ConstructionArea = 120
            };

            // Act
            var result = await _controller.PostProperty(dto);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdProperty = Assert.IsType<Property>(createdAtResult.Value);
            Assert.Equal("P-002", createdProperty.Number);
            Assert.Equal(2, await _context.Properties.CountAsync());
        }

        [Fact]
        public async Task PostProperty_InvalidOwner_ReturnsBadRequest()
        {
            // Arrange
            var dto = new PropertyCreateDto
            {
                PropertyTypeId = 1,
                OwnerId = 999, // ID inexistente
                Number = "P-003",
                Address = "Dirección Invalida",
                Area = 100
            };

            // Act
            var result = await _controller.PostProperty(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostProperty_InvalidPropertyType_ReturnsBadRequest()
        {
            // Arrange
            var dto = new PropertyCreateDto
            {
                PropertyTypeId = 999, // ID inexistente
                OwnerId = 1,
                Number = "P-004",
                Address = "Otra Dirección",
                Area = 200
            };

            // Act
            var result = await _controller.PostProperty(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task PutProperty_ValidUpdate_UpdatesProperty()
        {
            // Arrange
            var dto = new PropertyUpdateDto
            {
                Id = 1,
                PropertyTypeId = 2,
                OwnerId = 2,
                Number = "P-001-Modificado",
                Address = "Dirección Actualizada",
                Area = 120,
                ConstructionArea = 100
            };

            // Act
            var result = await _controller.PutProperty(1, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var updatedProperty = await _context.Properties.FindAsync(1);
            Assert.Equal("Dirección Actualizada", updatedProperty.Address);
            Assert.Equal(2, updatedProperty.OwnerId);
        }

        [Fact]
        public async Task PutProperty_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var dto = new PropertyUpdateDto
            {
                Id = 1,
                PropertyTypeId = 1,
                OwnerId = 1,
                Number = "P-001",
                Address = "Dirección",
                Area = 100
            };

            // Act
            var result = await _controller.PutProperty(2, dto); // ID diferente

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteProperty_ExistingId_DeletesProperty()
        {
            // Act
            var result = await _controller.DeleteProperty(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.Properties.FindAsync(1));
        }

        [Fact]
        public async Task DeleteProperty_NonExistingId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.DeleteProperty(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PutProperty_InvalidOwner_ReturnsBadRequest()
        {
            // Arrange
            var dto = new PropertyUpdateDto
            {
                Id = 1,
                OwnerId = 999, // ID inexistente
                PropertyTypeId = 1,
                Number = "P-001",
                Address = "Dirección",
                Area = 100
            };

            // Act
            var result = await _controller.PutProperty(1, dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutProperty_InvalidPropertyType_ReturnsBadRequest()
        {
            // Arrange
            var dto = new PropertyUpdateDto
            {
                Id = 1,
                PropertyTypeId = 999, // ID inexistente
                OwnerId = 1,
                Number = "P-001",
                Address = "Dirección",
                Area = 100
            };

            // Act
            var result = await _controller.PutProperty(1, dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}