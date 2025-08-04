using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.SetMarketData.Infrastructure.Exceptions;
using Pi.SetMarketData.Infrastructure.Interfaces.TimescaleEf;
using Pi.SetMarketData.Infrastructure.Services.TimescaleEf;

namespace Pi.SetMarketData.Infrastructure.Tests.Services.TimescaleEf
{
    public class TimescaleServiceTests
    {
        private readonly Mock<ITimescaleRepository<TestEntity>> _mockRepository;
        private readonly Mock<ILogger<TimescaleService<TestEntity>>> _mockLogger;
        private readonly TimescaleService<TestEntity> _service;

        public TimescaleServiceTests()
        {
            _mockRepository = new Mock<ITimescaleRepository<TestEntity>>();
            _mockLogger = new Mock<ILogger<TimescaleService<TestEntity>>>();
            _service = new TimescaleService<TestEntity>(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            // Arrange
            var entities = new List<TestEntity> { new TestEntity(), new TestEntity() };
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(entities, result);
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldThrowTimescaleServiceException_WhenRepositoryThrows()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("Repository error"));

            // Act & Assert
            await Assert.ThrowsAsync<TimescaleServiceException>(() => _service.GetAllAsync());
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Failed to get all entities")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEntity()
        {
            // Arrange
            var entity = new TestEntity { Id = 1 };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.Equal(entity, result);
            _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByFilterAsync_ShouldReturnFilteredEntity()
        {
            // Arrange
            var entity = new TestEntity { Id = 1, Name = "Test" };
            Expression<Func<TestEntity, bool>> filter = e => e.Name == "Test";
            _mockRepository.Setup(r => r.GetByFilterAsync(filter)).ReturnsAsync(entity);

            // Act
            var result = await _service.GetByFilterAsync(filter);

            // Assert
            Assert.Equal(entity, result);
            _mockRepository.Verify(r => r.GetByFilterAsync(filter), Times.Once);
        }

        [Fact]
        public async Task GetSelectedHighestValueAsync_ShouldReturnHighestValue()
        {
            // Arrange
            Expression<Func<TestEntity, bool>> filter = e => e.Name == "Test";
            Expression<Func<TestEntity, int>> keySelector = e => e.Id;
            Expression<Func<TestEntity, int>> selector = e => e.Id;
            _mockRepository.Setup(r => r.GetSelectedHighestValueAsync(filter, keySelector, selector)).ReturnsAsync(5);

            // Act
            var result = await _service.GetSelectedHighestValueAsync(filter, keySelector, selector);

            // Assert
            Assert.Equal(5, result);
            _mockRepository.Verify(r => r.GetSelectedHighestValueAsync(filter, keySelector, selector), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateEntity()
        {
            // Arrange
            var entity = new TestEntity();

            // Act
            await _service.CreateAsync(entity);

            // Assert
            _mockRepository.Verify(r => r.CreateAsync(entity), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEntity()
        {
            // Arrange
            var entity = new TestEntity { Id = 1 };

            // Act
            await _service.UpdateAsync(1, entity);

            // Assert
            _mockRepository.Verify(r => r.UpdateAsync(1, entity), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteEntity()
        {
            // Act
            await _service.DeleteAsync(1);

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task UpsertAsync_ShouldUpsertEntity()
        {
            // Arrange
            var entity = new TestEntity();
            var tableName = "table";
            var conflictKeys = new[] { "Id" };

            // Act
            await _service.UpsertAsync(entity, tableName, conflictKeys);

            // Assert
            _mockRepository.Verify(r => r.UpsertAsync(entity, tableName, conflictKeys), Times.Once);
        }
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}