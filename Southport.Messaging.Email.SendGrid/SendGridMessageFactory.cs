using System.Net.Http;
using Southport.Messaging.Email.Core;
using Southport.Messaging.Email.SendGrid.Interfaces;

namespace Southport.Messaging.Email.SendGrid
{
    public class SendGridMessageFactory : ISendGridMessageFactory
    {
        private readonly HttpClient _httpClient;
        private readonly ISendGridOptions _options;

        public SendGridMessageFactory(HttpClient httpClient, ISendGridOptions options)
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
