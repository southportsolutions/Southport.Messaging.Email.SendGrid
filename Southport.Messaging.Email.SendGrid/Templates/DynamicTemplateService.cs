using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Southport.Messaging.Email.SendGrid.Interfaces;
using Southport.Messaging.Email.SendGrid.Templates.Models;

namespace Southport.Messaging.Email.SendGrid.Templates
{
    public class DynamicTemplateService : IDynamicTemplateService
    {
        private readonly HttpClient _httpClient;
        private readonly SendGridOptions _options;

        public DynamicTemplateService(HttpClient httpClient, IOptions<SendGridOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;

            _httpClient.BaseAddress = new Uri("https://api.sendgrid.com/v3/templates/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        }

        public async Task<ResponseData<DynamicTemplateVersion>> GetTemplateVersion(string templateId, string versionId, CancellationToken cancellationToken)
        {
            try
            {
                var template = await _httpClient.GetFromJsonAsync<DynamicTemplateVersion>($"{templateId}/versions/{versionId}", cancellationToken); 
                return new ResponseData<DynamicTemplateVersion> { Data = template, StatusCode = 200 };
            }
            catch (HttpRequestException e)
            {
                return new ResponseData<DynamicTemplateVersion> { StatusCode = e.StatusCode == null ? null : (int)e.StatusCode, Message = e.Message };
            }
            
        }
    }

    public class ResponseData<T> : DynamicTemplateVersion
    {
        public T Data { get; set; }
        public int? StatusCode { get; set; }
        public string Message { get; set; }
    }
}
