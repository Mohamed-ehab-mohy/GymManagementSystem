namespace GymManagementSystem.BLL.Interfaces;

public interface IAttachmentService
{
    Task<string?> SaveFileAsync(string subFolder, int entityId, string fileName, Stream content);
    Task<bool> DeleteFileAsync(string relativePath);
    string GetContentType(string extension);
}
