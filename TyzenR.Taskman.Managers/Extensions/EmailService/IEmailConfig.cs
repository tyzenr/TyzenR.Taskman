namespace TyzenR.Investor.Managers.Extensions.EmailService
{
    public interface IEmailConfig
    {
        string EmailAdress { get; set; }
        int IncomingPort { get; set; }
        int OutgoingPort { get; set; }
        string Password { get; set; }
        string Server { get; set; }
    }
}