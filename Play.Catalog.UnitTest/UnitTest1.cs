using Play.Catelog.Service.Controllers;
using Play.Catelog.Service.Entities;
using Play.Common;
using Moq;
using MassTransit;
using Play.Catelog.Service.Dtos;
using Play.Catelog.Service;

namespace Play.Catalog.UnitTest 
{
    public class ItemsController_Tests
    {
        private readonly ItemsController _itemsController;
        private readonly Mock<IRepository<Item>> itemsRepository = new Mock<IRepository<Item>>();
        private readonly Mock<IPublishEndpoint> publishEndpoint = new Mock<IPublishEndpoint>();

        public ItemsController_Tests()
        {
            this._itemsController = new ItemsController(itemsRepository.Object, publishEndpoint.Object);
        }
        [Fact]
        public async Task GetByIdAsync_ShouldReturnCustomer()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var item = new Item
            {
                Id = itemId,
                Name = "potion",
                Description = "heals you",
                Price = 10,
                CreatedDate = DateTimeOffset.UtcNow
            };
            var itemDto = item.AsDto();

            //creates a mock of the GetAsync function in the mock repo
            itemsRepository.Setup(x => x.GetAsync(itemId)).ReturnsAsync(item);

            // Act
            //ItemsController itemsController = new ItemsController();
            var acutal = (await _itemsController.GetByIdAsync(itemId)).Value;

            // Assert
            Assert.Equal(item.Id, acutal.Id);

        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNothingWhenCustomerIsNull()
        {
            // Arrange
            itemsRepository.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);

            // Act
            var actual = _itemsController.GetByIdAsync(Guid.NewGuid());

            // Assert
            // Tests that nothing is returned
            Assert.Null(actual);

        }
    }
}