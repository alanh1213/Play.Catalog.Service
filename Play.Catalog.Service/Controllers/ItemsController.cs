using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Domain;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        //private static readonly List<ItemDto> items = new()
        //{
        //    new ItemDto(Guid.NewGuid(), "Small Potion", "Restores a small amount of HP", 5, DateTime.UtcNow),
        //    new ItemDto(Guid.NewGuid(), "Antidote", "Cures poison", 10, DateTime.UtcNow),
        //    new ItemDto(Guid.NewGuid(), "Bronze sword", "Cheap figthing weapon, deals a small amount of damage", 3, DateTime.UtcNow),
        //    new ItemDto(Guid.NewGuid(), "Leather body armor", "Decent protection for a decent price...", 50, DateTime.UtcNow)
        //};

        private readonly ItemsRepository itemsRepository = new();

        // GET /items
        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAsync()
        {
            var items = (await itemsRepository.GetAllAsync())
                        .Select(item => item.AsDto());

            return items;
        }

        // GET /items/12345 ----> id = 12345
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item is null) return NotFound();
            
            return item.AsDto();
        }

        // POST /items
        [HttpPost]
        public async Task<ActionResult<ItemDto>> Post(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTime.UtcNow
            };

            await itemsRepository.CreateAsync(item);

            return CreatedAtAction(nameof(GetByIdAsync), new {id = item.Id}, item);
        }

        // PUT /items/12345 ----> id = 12345
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await itemsRepository.GetAsync(id);

            if (existingItem is null) return NotFound();

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await itemsRepository.UpdateAsync(existingItem);

            return NoContent();
        }

        // DELETE /items/
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingItem = await itemsRepository.GetAsync(id);
            if (existingItem is null) return NotFound();


            await itemsRepository.RemoveAsync(id);

            return NoContent();
        }
    }
}
