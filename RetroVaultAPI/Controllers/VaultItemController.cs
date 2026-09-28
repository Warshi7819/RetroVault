using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using RetroVaultAPI.Data;
using RetroVaultAPI.Services;
using RetroVault.Shared;
using RetroVault.Shared.Models;


namespace RetroVaultAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaultItemController : ControllerBase
    {
        private readonly RetroVaultContext _context;
        private readonly LibraryFileService _library;

        private static readonly FileExtensionContentTypeProvider ContentTypes = new FileExtensionContentTypeProvider();

        public VaultItemController(RetroVaultContext context, LibraryFileService library)
        {
            _context = context;
            _library = library;
        }

        [HttpGet]
        public async Task<ActionResult<List<VaultItem>>> GetVaultItems()
        {
            return Ok(await _context.VaultItems.ToListAsync());
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<string>>> GetCategories()
        {
            var categories = await _context.VaultItems
                .Select(v => v.Category)
                .Where(c => c != null && c != "")
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet("systems")]
        public async Task<ActionResult<List<string>>> GetSystems()
        {
            var systems = await _context.VaultItems
                .Select(v => v.System)
                .Where(s => s != null && s != "")
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            return Ok(systems);
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
            existingItem.Sold = updatedItem.Sold;
            existingItem.SalePrice = updatedItem.SalePrice;
            existingItem.PriceChartingURL = updatedItem.PriceChartingURL;
            existingItem.PriceChartingLoosePrice = updatedItem.PriceChartingLoosePrice;
            existingItem.PriceChartingCompletePrice = updatedItem.PriceChartingCompletePrice;
            existingItem.PriceChartingLastUpdated = updatedItem.PriceChartingLastUpdated;


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
            var thumbnailsPath = Path.Combine(Environment.CurrentDirectory, "Thumbnails", item.Thumbnail);
            if (System.IO.File.Exists(thumbnailsPath))
            { 
                System.IO.File.Delete(thumbnailsPath);
            }

            // Delete the item's library folder (Audio, Documents, Images, Software, Videos) as well
            _library.DeleteItemDirectory(id);

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
            var thumbnailsPath = Path.Combine(Environment.CurrentDirectory, "Thumbnails");
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

        [HttpGet("{id}/files")]
        public async Task<ActionResult<List<VaultFile>>> GetVaultItemFiles(int id)
        {
            var item = await _context.VaultItems.FindAsync(id);
            if (item == null)
                return NotFound($"Vault item with ID {id} not found.");

            return Ok(_library.GetFiles(id));
        }

        [HttpGet("{id}/folders")]
        public async Task<ActionResult<List<VaultFolder>>> GetVaultItemFolders(int id)
        {
            var item = await _context.VaultItems.FindAsync(id);
            if (item == null)
                return NotFound($"Vault item with ID {id} not found.");

            return Ok(_library.GetFolders(id));
        }

        [HttpPost("{id}/files/{category}/folder")]
        public async Task<ActionResult<VaultFolder>> CreateVaultItemFolder(int id, string category, [FromForm] string path)
        {
            var item = await _context.VaultItems.FindAsync(id);
            if (item == null)
                return NotFound($"Vault item with ID {id} not found.");

            if (!LibraryFileService.IsValidCategory(category))
                return BadRequest($"Invalid category '{category}'. Expected one of: {string.Join(", ", LibraryFileService.Categories)}.");

            if (string.IsNullOrWhiteSpace(path))
                return BadRequest("No folder name provided.");

            if (!_library.TryGetFolderDirectory(id, category, path, out _))
                return BadRequest($"Invalid folder name '{path}'.");

            if (_library.FolderExists(id, category, path))
                return BadRequest("Folder already exists.");

            _library.CreateFolder(id, category, path);

            return Ok(new VaultFolder
            {
                Category = LibraryFileService.GetCategoryName(category),
                Path = path.Replace('\\', '/').Trim('/'),
                Name = path.Replace('\\', '/').Trim('/').Split('/').Last()
            });
        }

        [HttpDelete("{id}/files/{category}/folder")]
        public async Task<ActionResult> DeleteVaultItemFolder(int id, string category, [FromQuery] string path)
        {
            var item = await _context.VaultItems.FindAsync(id);
            if (item == null)
                return NotFound($"Vault item with ID {id} not found.");

            if (!_library.TryGetFolderDirectory(id, category, path, out var directory))
                return BadRequest("Invalid folder path.");

            if (!Directory.Exists(directory))
                return NotFound($"Folder '{path}' not found.");

            _library.DeleteFolder(id, category, path);
            return NoContent();
        }

        [HttpGet("{id}/files/{category}/{*filePath}")]
        public async Task<IActionResult> DownloadVaultItemFile(int id, string category, string filePath)
        {
            var item = await _context.VaultItems.FindAsync(id);
            if (item == null)
                return NotFound($"Vault item with ID {id} not found.");

            if (!_library.TryGetFilePath(id, category, filePath, out var fullPath))
                return BadRequest("Invalid file path.");

            if (!System.IO.File.Exists(fullPath))
                return NotFound($"File '{filePath}' not found.");

            if (!ContentTypes.TryGetContentType(fullPath, out var contentType))
                contentType = "application/octet-stream";

            var fileName = Path.GetFileName(fullPath);

            if (!LibraryFileService.IsInlineType(fullPath))
            {
                Response.Headers["Content-Disposition"] = $"attachment; filename=\"{fileName}\"";
            }

            return PhysicalFile(fullPath, contentType, enableRangeProcessing: true);
        }

        [HttpPost("{id}/files/{category}")]
        public async Task<ActionResult<List<VaultFile>>> UploadVaultItemFiles(int id, string category, [FromForm] List<IFormFile>? files, [FromForm] string? subfolder)
        {
            var item = await _context.VaultItems.FindAsync(id);
            if (item == null)
                return NotFound($"Vault item with ID {id} not found.");

            if (!LibraryFileService.IsValidCategory(category))
                return BadRequest($"Invalid category '{category}'. Expected one of: {string.Join(", ", LibraryFileService.Categories)}.");

            if (files == null || files.Count == 0)
                return BadRequest("No file uploaded.");

            var saved = new List<VaultFile>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                    continue;

                if (!_library.TryGetUploadPath(id, category, subfolder, file.FileName, out _))
                    return BadRequest($"Invalid file name '{file.FileName}'.");

                using (var stream = file.OpenReadStream())
                {
                    saved.Add(await _library.SaveFileAsync(id, category, subfolder, file.FileName, stream, file.Length));
                }
            }

            if (saved.Count == 0)
                return BadRequest("No file uploaded.");

            return Ok(saved);
        }

        [HttpDelete("{id}/files")]
        public async Task<ActionResult> DeleteVaultItemFile(int id, [FromQuery] string category, [FromQuery] string path)
        {
            var item = await _context.VaultItems.FindAsync(id);
            if (item == null)
                return NotFound($"Vault item with ID {id} not found.");

            if (!_library.TryGetFilePath(id, category, path, out var fullPath))
                return BadRequest("Invalid file path.");

            if (!System.IO.File.Exists(fullPath))
                return NotFound($"File '{path}' not found.");

            System.IO.File.Delete(fullPath);
            return NoContent();
        }
    }
}
