namespace GymManagementSystem.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public int StatusCode { get; set; } = 500;
    public string Title { get; set; } = "An Error Occurred";
    public string Message { get; set; } = "Something went wrong while processing your request.";
    public string? ExceptionDetails { get; set; }

    public string StatusIcon => StatusCode switch
    {
        404 => "bi-search",
        403 => "bi-shield-lock",
        401 => "bi-lock",
        500 => "bi-bug",
        _   => "bi-exclamation-triangle"
    };

    public string StatusColor => StatusCode switch
    {
        404 => "#6366f1",
        403 => "#f59e0b",
        401 => "#f59e0b",
        500 => "#ef4444",
        _   => "#8b5cf6"
    };
}
