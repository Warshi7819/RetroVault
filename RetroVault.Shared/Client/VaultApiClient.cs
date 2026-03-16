using RetroVault.Shared.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace RetroVault.Shared
{
    public class VaultApiClient
    {
        private readonly HttpClient _http;

        public VaultApiClient(HttpClient http)
        {
            _http = http;
        }

        // GET by ID
        public async Task<VaultItem?> GetVaultItemAsync(int id)
        {
            var response = await _http.GetAsync($"VaultItem/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<VaultItem>();
        }

        // SEARCH (name, system, category)
        public async Task<PagedResult<VaultItem>> SearchVaultItemsAsync(
            string? name = null,
            string? system = null,
            string? category = null,
            int page = 1,
            int pageSize = 10)
        {
            var query = new List<string>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                query.Add($"name={Uri.EscapeDataString(name)}");
            }
            if (!string.IsNullOrWhiteSpace(system) && system != "All")
            {
                system = system.Trim();
                query.Add($"system={Uri.EscapeDataString(system)}");
            }
            if (!string.IsNullOrWhiteSpace(category) && category != "All")
            {
                category = category.Trim();
                query.Add($"category={Uri.EscapeDataString(category)}");
            }
            query.Add($"page={page}");
            query.Add($"pageSize={pageSize}");

            string url = "VaultItem/search";
            if (query.Count > 0)
                url += "?" + string.Join("&", query);

            using var response = await _http.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new PagedResult<VaultItem>();
            }

            response.EnsureSuccessStatusCode(); // Throws for other errors (500, 401, etc.)
            return await response.Content.ReadFromJsonAsync<PagedResult<VaultItem>>() ?? new PagedResult<VaultItem>();
        }

        // CREATE
        public async Task<VaultItem?> CreateVaultItemAsync(VaultItem item)
        {
            var response = await _http.PostAsJsonAsync("VaultItem", item);

            if (!response.IsSuccessStatusCode)
            {
                // Read error details and throw
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to create VaultItem. Status: {response.StatusCode}. Error: {error}"); 
            }

            return await response.Content.ReadFromJsonAsync<VaultItem>();
        }

        // UPDATE
        public async Task<bool> UpdateVaultItemAsync(int id, VaultItem item)
        {
            var response = await _http.PutAsJsonAsync($"VaultItem/{id}", item);
            return response.IsSuccessStatusCode;
        }

        // DELETE
        public async Task<bool> DeleteVaultItemAsync(int id)
        {
            var response = await _http.DeleteAsync($"VaultItem/{id}");
            return response.IsSuccessStatusCode;
        }

        // UPLOAD THUMBNAIL
        public async Task<bool> UploadThumbnail(int id, string path)
        {
            var filename = Path.GetFileName(path);
            var form = new MultipartFormDataContent();
            form.Add(new StreamContent(File.OpenRead(path)), "file", filename);
            
            var response = await _http.PostAsync($"VaultItem/{id}/thumbnail", form);
            return response.IsSuccessStatusCode;
        }
    }
}
