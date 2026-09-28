using RetroVault.Shared;

namespace RetroVaultWebApp.Services
{
    public class LibraryProxyService
    {
        private readonly VaultApiClient _api;

        public LibraryProxyService(VaultApiClient api)
        {
            _api = api;
        }

        public async Task ProxyFileAsync(HttpContext context, int id, string category, string path)
        {
            var range = context.Request.Headers["Range"].ToString();
            var ifNoneMatch = context.Request.Headers["If-None-Match"].ToString();

            using var response = await _api.GetFileAsync(
                id,
                category,
                path,
                string.IsNullOrEmpty(range) ? null : range,
                string.IsNullOrEmpty(ifNoneMatch) ? null : ifNoneMatch);

            context.Response.StatusCode = (int)response.StatusCode;

            var notModified = response.StatusCode == System.Net.HttpStatusCode.NotModified;
            if (!response.IsSuccessStatusCode && !notModified)
            {
                return;
            }

            if (response.Content.Headers.ContentType is { } contentType)
                context.Response.ContentType = contentType.ToString();

            if (response.Content.Headers.ContentLength is { } contentLength)
                context.Response.ContentLength = contentLength;

            if (response.Content.Headers.ContentRange is { } contentRange)
                context.Response.Headers["Content-Range"] = contentRange.ToString();

            if (response.Content.Headers.ContentDisposition is { } contentDisposition)
                context.Response.Headers["Content-Disposition"] = contentDisposition.ToString();

            if (response.Content.Headers.LastModified is { } lastModified)
                context.Response.Headers["Last-Modified"] = lastModified.ToString("R");

            if (response.Headers.ETag is { } etag)
                context.Response.Headers["ETag"] = etag.ToString();

            if (response.Headers.AcceptRanges is { } acceptRanges)
                context.Response.Headers["Accept-Ranges"] = string.Join(", ", acceptRanges);

            if (notModified)
                return;

            await response.Content.CopyToAsync(context.Response.Body);
        }
    }
}
