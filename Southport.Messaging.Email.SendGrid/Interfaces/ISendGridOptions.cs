namespace Southport.Messaging.Email.SendGrid.Interfaces
{
    public interface ISendGridOptions
    {
        string ApiKey { get; set; }
        string TestEmailAddresses { get; set; }
        bool UseTestMode { get; set; }
    }

    public class SendGridOptions : ISendGridOptions
    {
        public string ApiKey { get; set; }
        public string TestEmailAddresses { get; set; }
        public bool UseTestMode { get; set; }
    }
}