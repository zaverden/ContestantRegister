namespace ContestantRegister.Services.InfrastructureServices
{
    public interface IUrlHelper
    {
        string Action(string action, string controller, object values, string protocol);
    }
}
