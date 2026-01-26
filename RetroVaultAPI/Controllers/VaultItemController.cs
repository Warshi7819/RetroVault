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

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<VaultItem>>> SearchVaultItems(
        [FromQuery] string? name,
        [FromQuery] string? system,
        [FromQuery] string? category)
        {
            IQueryable<VaultItem> query = _context.VaultItems;

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(v => EF.Functions.Like(v.Name, $"%{name}%"));
            }

            if (!string.IsNullOrWhiteSpace(system))
            {
                query = query.Where(v => v.System == system);
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(v => v.Category == category);
            }

            var results = await query.ToListAsync();

            if (results.Count == 0)
            {
                return NotFound("No vault items matched the search criteria.");
            }

            return Ok(results);
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
            existingItem.Region = updatedItem.Region;
            existingItem.Developer = updatedItem.Developer;
            existingItem.Publisher = updatedItem.Publisher;
            existingItem.Year = updatedItem.Year;
            existingItem.AcquiredDate = updatedItem.AcquiredDate;
            existingItem.Completeness = updatedItem.Completeness;   
            existingItem.AcquiredFrom = updatedItem.AcquiredFrom;
            existingItem.StorageLocation = updatedItem.StorageLocation;
            existingItem.PurchasePrice = updatedItem.PurchasePrice;
            existingItem.Currency = updatedItem.Currency;


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
