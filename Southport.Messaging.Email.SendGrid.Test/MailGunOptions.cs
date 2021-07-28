
namespace Southport.Messaging.Email.MailGun.Test
{
    public class MailGunOptions : IMailGunOptions
    {
        public string ApiKey { get; set; }
        public string Domain { get; set; }
        public string TestEmailAddresses { get; set; }
    }
}
