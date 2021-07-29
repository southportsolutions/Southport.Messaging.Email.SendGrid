
using Southport.Messaging.Email.SendGrid.Interfaces;

namespace Southport.Messaging.Email.SendGrid.Test
{
    public class EmailOptions : ISendGridOptions
    {
        public string ApiKey { get; set; }
        public string TestEmailAddresses { get; set; }
        public bool UseTestMode { get; set; }
    }
}
