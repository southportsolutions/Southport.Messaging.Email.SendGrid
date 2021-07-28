namespace Southport.Messaging.Email.SendGrid.Interfaces
{
    public interface ISendGridOptions
    {
        string ApiKey { get; set; }
        string Domain { get; set; }
        string TestEmailAddresses { get; set; }
    }
}