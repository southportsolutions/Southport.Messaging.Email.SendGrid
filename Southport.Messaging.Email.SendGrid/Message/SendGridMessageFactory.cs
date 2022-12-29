using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Southport.Messaging.Email.Core;
using Southport.Messaging.Email.SendGrid.Interfaces;
using Southport.Messaging.Email.SendGrid.Message.Interfaces;

namespace Southport.Messaging.Email.SendGrid.Message
{
    public class SendGridMessageFactory : ISendGridMessageFactory
    {
        private readonly HttpClient _httpClient;
        private readonly ISendGridOptions _options;

        public SendGridMessageFactory(HttpClient httpClient, IOptions<SendGridOptions> options)
        {
            _httpClient = httpClient;
            
            _httpClient.BaseAddress = new Uri("https://api.sendgrid.com/v3/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.Value.ApiKey);

            _options = options.Value;
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
