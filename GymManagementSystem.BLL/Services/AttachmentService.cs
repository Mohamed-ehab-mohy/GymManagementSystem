using GymManagementSystem.BLL.Interfaces;

namespace GymManagementSystem.BLL.Services;

public class AttachmentService : IAttachmentService
{
    private readonly string _webRootPath;

    public AttachmentService(string webRootPath)
    {
        _webRootPath = webRootPath;
    }

    public async Task<string?> SaveFileAsync(string subFolder, int entityId, string fileName, Stream content)
    {
        var uploadsDir = Path.Combine(_webRootPath, subFolder);
        Directory.CreateDirectory(uploadsDir);

        var ext = Path.GetExtension(fileName);
        var safeName = $"{subFolder}_{entityId}_{DateTime.Now:yyyyMMdd_HHmmss}{ext}";
        var filePath = Path.Combine(uploadsDir, safeName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await content.CopyToAsync(stream);
        }

        return $"/{subFolder}/{safeName}";
    }

    public Task<bool> DeleteFileAsync(string relativePath)
    {
        var fullPath = Path.Combine(_webRootPath, relativePath.TrimStart('/'));
        if (!System.IO.File.Exists(fullPath))
            return Task.FromResult(false);

        System.IO.File.Delete(fullPath);
        return Task.FromResult(true);
    }

    public string GetContentType(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream"
        };
    }
}
