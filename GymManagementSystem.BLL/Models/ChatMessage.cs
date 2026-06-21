namespace GymManagementSystem.BLL.Models;

public class ChatMessage
{
    public string Role { get; set; } = null!;
    public string Content { get; set; } = null!;

    public ChatMessage() { }

    public ChatMessage(string role, string content)
    {
        Role = role;
        Content = content;
    }
}