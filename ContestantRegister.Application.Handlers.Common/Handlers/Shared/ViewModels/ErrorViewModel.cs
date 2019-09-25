namespace ContestantRegister.Application.Handlers.Common.Handlers.Shared.ViewModels
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public string Message { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}