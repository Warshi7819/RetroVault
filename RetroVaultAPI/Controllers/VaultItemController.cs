using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetroVaultAPI.Data;
using RetroVaultAPI.Models;


namespace RetroVaultAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaultItemController : ControllerBase
    {
        //private static List<VaultItem> vaultItems = new List<VaultItem>
        //{
        //    new VaultItem { Id = 1, Name = "Super Mario Bros", Category = "Game", System = "NES", Year = 1985 },
        //    new VaultItem { Id = 2, Name = "The Legend of Zelda", Category = "Game", System = "NES", Year = 1986 }
        //};

        private readonly RetroVaultContext _context;
        public VaultItemController(RetroVaultContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<VaultItem>>> GetVaultItems()
        {
            return Ok(await _context.VaultItems.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VaultItem>> GetVaultItem(int id)
        {
            var item = await _context.VaultItems.FindAsync(id);
            if (item == null)
            {
                return NotFound($"Vault item with ID {id} not found.");
            }
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<VaultItem>> CreateVaultItem([FromBody] VaultItem newItem)
        {
            if (newItem == null)
            {
                return BadRequest("Invalid vault item data.");
            }
            
            _context.VaultItems.Add(newItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetVaultItem), new { id = newItem.Id }, newItem);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<VaultItem>> UpdateVaultItem(int id, [FromBody] VaultItem updatedItem)
        {
            var existingItem = await _context.VaultItems.FindAsync(id);
            if (existingItem == null)
            {
                return NotFound($"Vault item with ID {id} not found.");
            }
            if (updatedItem == null)
            {
                return BadRequest("Invalid vault item data.");
            }

            existingItem.Name = updatedItem.Name;
            existingItem.Description = updatedItem.Description;
            existingItem.Category = updatedItem.Category;
            existingItem.System = updatedItem.System;
            existingItem.Developer = updatedItem.Developer;
            existingItem.Publisher = updatedItem.Publisher;
            existingItem.Year = updatedItem.Year;
            existingItem.PhysicalLocation = updatedItem.PhysicalLocation;
            existingItem.Thumbnail = updatedItem.Thumbnail;
            existingItem.ImageFolder = updatedItem.ImageFolder;
            existingItem.VideoFolder = updatedItem.VideoFolder;
            existingItem.DocumentationFolder = updatedItem.DocumentationFolder;
            existingItem.Price = updatedItem.Price;
            existingItem.Currencty = updatedItem.Currencty;
            
            await _context.SaveChangesAsync();
            return Ok(existingItem);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVaultItem(int id)
        {
            var item = await _context.VaultItems.FindAsync(id);
            if (item == null)
            {
                return NotFound($"Vault item with ID {id} not found.");
            }

            _context.VaultItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
