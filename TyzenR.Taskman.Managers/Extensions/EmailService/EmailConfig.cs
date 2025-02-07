namespace TyzenR.Investor.Managers.Extensions.EmailService
{
    public class EmailConfig : IEmailConfig
    {
        public string Server { get; set; }
        public string EmailAdress { get; set; }
        public string Password { get; set; }
        public int IncomingPort { get; set; }
        public int OutgoingPort { get; set; }
    }
}
