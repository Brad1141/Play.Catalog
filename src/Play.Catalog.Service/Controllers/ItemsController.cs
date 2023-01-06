using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;
using Play.Catelog.Service.Dtos;
using Play.Catelog.Service.Entities;
using Play.Common;

namespace Play.Catelog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        //Example of dependency injection
        /*

        notice how the itemsController doesn't intiate the IItemsRespository interface
        instead the interface is passed in as a parameter

        the reason we are using an interface as opposed to a class is because of the dependancy inversion principle
        which states that code should only depend on abstractions

        also, because we are using interfaces, we are decouping the class from the dependancy, this means that our class
        logic will never change
        */
        private readonly IRepository<Item> itemsRepository;
        
        private readonly IPublishEndpoint publishEndpoint;

        public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
        {
            this.itemsRepository = itemsRepository;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {

            var items = (await itemsRepository.GetAllAsync())
                        .Select(item => item.AsDto());
            
            return Ok(items);
        }

        // GET /items/{id}
        [HttpGet("{id}")]
        // ActionResult<ItemDto> allows us to return an action result (like "NotFound()") or an ItemDto record
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            // return NotFound action result if item is null
            if (item == null)
            {
                return NotFound();
            }

            return item.AsDto();
        }

        // POST /items
        // Example of an HTTP POST
        // In a post request, we want to return an action result so our controller knows how we did
        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await itemsRepository.CreateAsync(item);

            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

            //creates a createdAtActionResult object which contains the status code, and the location of the new resource
            //which is the url of the "Post" method with the "id" parameter
            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        // PUT /items/{id}
        [HttpPut("{id}")]
        // you don't normally return anything in a put operation
        // when calling this function, put the UpdateItemDto record in the request body
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await itemsRepository.GetAsync(id);

            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await itemsRepository.UpdateAsync(existingItem);

            await publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));

            // return nothing
            return NoContent();
        }

        // Delete /items/{id}
        [HttpDelete("{id}")]
        public async  Task<IActionResult> DeleteAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            await itemsRepository.RemoveAsync(item.Id);

            await publishEndpoint.Publish(new CatalogItemDeleted(item.Id));

            return NoContent();
        }
    }
}

