using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GymManagementSystem.BLL.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GymManagementSystem.BLL.Services;

public class AttachmentService : IAttachmentService
{
    private readonly Cloudinary _cloudinary;

    public AttachmentService(IConfiguration configuration)
    {
        var cloudName = configuration["Cloudinary:CloudName"];
        var apiKey = configuration["Cloudinary:ApiKey"];
        var apiSecret = configuration["Cloudinary:ApiSecret"];

        if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            throw new InvalidOperationException("Cloudinary credentials are missing from configuration.");

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string?> SaveFileAsync(string subFolder, int entityId, string fileName, Stream content)
    {
        var ext = Path.GetExtension(fileName);
        var publicId = $"{subFolder}/{subFolder}_{entityId}_{DateTime.Now:yyyyMMdd_HHmmss}";

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, content),
            PublicId = publicId,
            Overwrite = true,
            UseFilename = false,
        };

        var result = await _cloudinary.UploadAsync(uploadParams);
        return result.StatusCode == System.Net.HttpStatusCode.OK
            ? result.SecureUrl.ToString()
            : null;
    }

    public async Task<bool> DeleteFileAsync(string relativePath)
    {
        var publicId = ExtractPublicId(relativePath);
        if (string.IsNullOrEmpty(publicId))
            return false;

        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        return result.StatusCode == System.Net.HttpStatusCode.OK;
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

    private static string? ExtractPublicId(string url)
    {
        var uri = new Uri(url);
        var segments = uri.AbsolutePath.Split('/').ToList();
        var uploadIdx = segments.FindLastIndex(s => s == "upload");
        if (uploadIdx < 0 || uploadIdx + 1 >= segments.Count)
            return null;

        var publicIdWithExt = string.Join("/", segments.Skip(uploadIdx + 2));
        var lastDot = publicIdWithExt.LastIndexOf('.');
        return lastDot > 0 ? publicIdWithExt[..lastDot] : publicIdWithExt;
    }
}
