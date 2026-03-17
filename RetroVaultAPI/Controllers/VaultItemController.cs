using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetroVaultAPI.Data;
using RetroVault.Shared;
using RetroVault.Shared.Models;


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
        public async Task<ActionResult<PagedResult<VaultItem>>> SearchVaultItems(
        [FromQuery] string? name,
        [FromQuery] string? system,
        [FromQuery] string? category,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            IQueryable<VaultItem> query = _context.VaultItems;

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(v => EF.Functions.Like(v.Name, $"%{name}%"));

            if (!string.IsNullOrWhiteSpace(system))
                query = query.Where(v => v.System == system);

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(v => v.Category == category);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(v => v.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<VaultItem>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(result);
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

            // If thumbnail uploaded, delete that as well
            var thumbnailsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Thumbnails", item.Thumbnail);
            if (System.IO.File.Exists(thumbnailsPath))
            { 
                System.IO.File.Delete(thumbnailsPath);
            }

            // Delete item from DB
            _context.VaultItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/thumbnail")]
        public async Task<IActionResult> UploadThumbnail(int id, IFormFile file)
        {
            var item = await _context.VaultItems.FindAsync(id);
            if (item == null)
                return NotFound($"Vault item with ID {id} not found.");

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Ensure folder exists
            var thumbnailsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Thumbnails");
            if (!Directory.Exists(thumbnailsPath))
                Directory.CreateDirectory(thumbnailsPath);

            // Create unique filename
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{id}{extension}";
            var filePath = Path.Combine(thumbnailsPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            //Store the filename in the DB
            item.Thumbnail = fileName;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thumbnail uploaded successfully.", fileName });
        }
    }
}
