namespace ContestantRegister.Services.InfrastructureServices
{
    public interface ICurrentUserService
    {
        string Email { get; }
        string Id { get; }
        bool IsAuthenticated { get; }
    }
}
