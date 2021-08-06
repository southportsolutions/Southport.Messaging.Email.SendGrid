using System.Net.Http;
using Southport.Messaging.Email.Core;
using Southport.Messaging.Email.SendGrid.HttpClients;
using Southport.Messaging.Email.SendGrid.Interfaces;
using Southport.Messaging.Email.SendGrid.Message.Interfaces;

namespace Southport.Messaging.Email.SendGrid.Message
{
    public class SendGridMessageFactory : ISendGridMessageFactory
    {
        private readonly ISendGridHttpClient _httpClient;
        private readonly ISendGridOptions _options;

        public SendGridMessageFactory(ISendGridHttpClient httpClient, ISendGridOptions options)
        {
            _httpClient = httpClient;
            _options = options;
        }


        public ISendGridMessage Create()
        {
            return new SendGridMessage(_httpClient, _options);
        }

        IEmailMessageCore IEmailMessageFactory.Create()
        {
            return Create();
        }
    }
}
