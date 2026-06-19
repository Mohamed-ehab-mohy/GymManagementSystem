namespace GymManagementSystem.DAL.Interceptors;

public interface ICurrentUserService
{
    string UserId { get; }
    string UserEmail { get; }
    bool IsAuthenticated { get; }
}
