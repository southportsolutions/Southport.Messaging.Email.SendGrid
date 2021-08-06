using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Southport.Messaging.Email.SendGrid.Interfaces;

namespace Southport.Messaging.Email.SendGrid.HttpClients
{
    public class SendGridHttpClient : ISendGridHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ISendGridOptions _options;

        public SendGridHttpClient(HttpClient httpClient, ISendGridOptions options)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.sendgrid.com/v3/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);
            _options = options;
        }

        public async Task<HttpResponseMessage> SendAsync(global::SendGrid.Helpers.Mail.SendGridMessage message, CancellationToken cancellationToken)
        {
            var json = message.Serialize();
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync("mail/send", stringContent, cancellationToken);
        }

        public async Task<string> GetBatchIdAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsync("mail/batch", null, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var batch = JsonConvert.DeserializeObject<SendGridBatchResponse>(responseString);
                return batch.BatchId;
            }

            return null;
        }
    }

    public class SendGridBatchResponse
    {
        [JsonProperty("batch_id")]
        public string BatchId { get; set; }
    }
}
