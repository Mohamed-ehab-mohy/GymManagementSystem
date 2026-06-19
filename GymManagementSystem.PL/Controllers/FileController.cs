using GymManagementSystem.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers;

[Route("uploads")]
[Authorize]
public class FileController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly IAttachmentService _attachmentService;

    public FileController(IWebHostEnvironment env, IAttachmentService attachmentService)
    {
        _env = env;
        _attachmentService = attachmentService;
    }

    [HttpGet("{filename}")]
    public IActionResult Serve(string filename)
    {
        var filePath = Path.Combine(_env.ContentRootPath, "App_Data", "uploads", filename);

        if (!System.IO.File.Exists(filePath))
            return NotFound();

        var ext = Path.GetExtension(filename);
        var contentType = _attachmentService.GetContentType(ext);

        return PhysicalFile(filePath, contentType);
    }
}
