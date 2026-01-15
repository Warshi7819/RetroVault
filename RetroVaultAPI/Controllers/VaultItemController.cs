using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetroVaultAPI.Models;


namespace RetroVaultAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaultItemController : ControllerBase
    {
        private static List<VaultItem> vaultItems = new List<VaultItem>
        {
            new VaultItem { Id = 1, Name = "Super Mario Bros", Category = "Game", System = "NES", Year = 1985 },
            new VaultItem { Id = 2, Name = "The Legend of Zelda", Category = "Game", System = "NES", Year = 1986 }
        };


        [HttpGet]
        public ActionResult<List<VaultItem>> GetVaultItems()
        {
            if (vaultItems == null || !vaultItems.Any())
            {
                return NotFound("No vault items found.");
            }
            return Ok(vaultItems);
        }

        [HttpGet("{id}")]
        public ActionResult<VaultItem> GetVaultItem(int id)
        {
            var item = vaultItems.FirstOrDefault(v => v.Id == id);
            if (item == null)
            {
                return NotFound($"Vault item with ID {id} not found.");
            }
            return Ok(item);
        }

        [HttpPost]
        public ActionResult<VaultItem> CreateVaultItem([FromBody] VaultItem newItem)
        {
            if (newItem == null)
            {
                return BadRequest("Invalid vault item data.");
            }
            newItem.Id = vaultItems.Max(v => v.Id) + 1;
            vaultItems.Add(newItem);
            return CreatedAtAction(nameof(GetVaultItem), new { id = newItem.Id }, newItem);
        }

        [HttpPut("{id}")]
        public ActionResult<VaultItem> UpdateVaultItem(int id, [FromBody] VaultItem updatedItem)
        {
            var existingItem = vaultItems.FirstOrDefault(v => v.Id == id);
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
            return Ok(existingItem);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteVaultItem(int id)
        {
            var item = vaultItems.FirstOrDefault(v => v.Id == id);
            if (item == null)
            {
                return NotFound($"Vault item with ID {id} not found.");
            }
            vaultItems.Remove(item);
            return NoContent();
        }
    }
}
