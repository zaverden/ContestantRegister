namespace ContestantRegister.Services.InfrastructureServices
{
    public interface ICurrentUserService
    {
        bool IsAdmin { get; }
        string Email { get; }
        string Id { get; }
        bool IsAuthenticated { get; }
    }
}
