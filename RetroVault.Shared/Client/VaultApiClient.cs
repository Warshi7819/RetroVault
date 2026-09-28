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


        // GET ALL SYSTEMS
        public async Task<List<string>> GetSystemsAsync()
        {
            var response = await _http.GetAsync("VaultItem/systems");
            if (!response.IsSuccessStatusCode)
                return new List<string>();
            return await response.Content.ReadFromJsonAsync<List<string>>() ?? new List<string>();
        }

        // GET ALL CATEGORIES
        public async Task<List<string>> GetCategoriesAsync()
        {
            var response = await _http.GetAsync("VaultItem/categories");
            if (!response.IsSuccessStatusCode)
                return new List<string>();
            return await response.Content.ReadFromJsonAsync<List<string>>() ?? new List<string>();
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

        // UPLOAD THUMBNAIL (from file path)
        public async Task<bool> UploadThumbnail(int id, string path)
        {
            var filename = Path.GetFileName(path);
            var form = new MultipartFormDataContent();
            form.Add(new StreamContent(File.OpenRead(path)), "file", filename);
            
            var response = await _http.PostAsync($"VaultItem/{id}/thumbnail", form);
            return response.IsSuccessStatusCode;
        }

        // UPLOAD THUMBNAIL (from stream)
        public async Task<bool> UploadThumbnailAsync(int id, Stream stream, string fileName)
        {
            var form = new MultipartFormDataContent();
            form.Add(new StreamContent(stream), "file", fileName);

            var response = await _http.PostAsync($"VaultItem/{id}/thumbnail", form);
            return response.IsSuccessStatusCode;
        }

        // GET ITEM FILES
        public async Task<List<VaultFile>> GetFilesAsync(int id)
        {
            var response = await _http.GetAsync($"VaultItem/{id}/files");
            if (!response.IsSuccessStatusCode)
                return new List<VaultFile>();
            return await response.Content.ReadFromJsonAsync<List<VaultFile>>() ?? new List<VaultFile>();
        }

        // GET ITEM FILE (caller disposes the response and its stream)
        public async Task<HttpResponseMessage> GetFileAsync(int id, string category, string path, string? range = null, string? ifNoneMatch = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"VaultItem/{id}/files/{Uri.EscapeDataString(category)}/{EscapePath(path)}");

            if (!string.IsNullOrEmpty(range))
                request.Headers.TryAddWithoutValidation("Range", range);
            if (!string.IsNullOrEmpty(ifNoneMatch))
                request.Headers.TryAddWithoutValidation("If-None-Match", ifNoneMatch);

            return await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        }

        // UPLOAD ITEM FILE (overwrites an existing file with the same name)
        public async Task<List<VaultFile>?> UploadFileAsync(int id, string category, Stream stream, string fileName, string? subfolder = null)
        {
            var form = new MultipartFormDataContent();
            form.Add(new StreamContent(stream), "files", fileName);
            if (!string.IsNullOrWhiteSpace(subfolder))
                form.Add(new StringContent(subfolder), "subfolder");

            var response = await _http.PostAsync($"VaultItem/{id}/files/{Uri.EscapeDataString(category)}", form);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<List<VaultFile>>();
        }

        // GET ITEM FOLDERS
        public async Task<List<VaultFolder>> GetFoldersAsync(int id)
        {
            var response = await _http.GetAsync($"VaultItem/{id}/folders");
            if (!response.IsSuccessStatusCode)
                return new List<VaultFolder>();
            return await response.Content.ReadFromJsonAsync<List<VaultFolder>>() ?? new List<VaultFolder>();
        }

        // CREATE ITEM FOLDER (path relative to the category, nested allowed)
        public async Task<VaultFolder?> CreateFolderAsync(int id, string category, string path)
        {
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(path), "path");

            var response = await _http.PostAsync($"VaultItem/{id}/files/{Uri.EscapeDataString(category)}/folder", form);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<VaultFolder>();
        }

        // DELETE ITEM FOLDER (recursive)
        public async Task<bool> DeleteFolderAsync(int id, string category, string path)
        {
            var url = $"VaultItem/{id}/files/{Uri.EscapeDataString(category)}/folder?path={Uri.EscapeDataString(path)}";
            var response = await _http.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }

        // DELETE ITEM FILE
        public async Task<bool> DeleteFileAsync(int id, string category, string path)
        {
            var url = $"VaultItem/{id}/files?category={Uri.EscapeDataString(category)}&path={Uri.EscapeDataString(path)}";
            var response = await _http.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }

        private static string EscapePath(string path)
        {
            return string.Join("/", path.Split('/', '\\').Select(Uri.EscapeDataString));
        }
    }
}
