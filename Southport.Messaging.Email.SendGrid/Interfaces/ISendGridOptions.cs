namespace Southport.Messaging.Email.SendGrid.Interfaces
{
    public interface ISendGridOptions
    {
        string ApiKey { get; set; }
        string TestEmailAddresses { get; set; }
        bool UseTestMode { get; set; }
    }
}