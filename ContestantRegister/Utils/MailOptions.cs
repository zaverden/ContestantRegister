namespace ContestantRegister.Utils
{
    public class MailOptions
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FromName { get; set; }
        public string SecurityToken { get; set; }
    }
}
