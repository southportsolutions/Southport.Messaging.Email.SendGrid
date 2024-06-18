using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Southport.Messaging.Email.SendGrid.Templates;
using Xunit;

namespace Southport.Messaging.Email.SendGrid.Test.Templates
{
    public class DynamicTemplateServiceTests : IDisposable
    {
        private const string TemplateId = "d-a043251c72e644888788ec6eb2fb6973";
        private readonly HttpClient _httpClient;

        private readonly DynamicTemplateService _service;

        public DynamicTemplateServiceTests()
        {
            var options = Startup.GetOptions();
            _httpClient = new HttpClient();

            _service = new DynamicTemplateService(_httpClient, Options.Create(options));
        }

        [Fact]
        public async Task GetTemplate()
        {
            var versionId = "b304e4c1-bd50-4e0d-adb2-7ad57c723839";
            var responseData = await _service.GetTemplateVersion(TemplateId, versionId, default);

            var template = responseData.Data;
            Assert.NotNull(template);
            Assert.Equal(versionId, template.Id);
            Assert.Equal(TemplateId, template.TemplateId);
            Assert.Equal(versionId, template.Id);
            Assert.Equal("Test Version", template.Name);
            Assert.NotNull(template.Subject);
            Assert.NotNull(template.HtmlContent);
            Assert.NotNull(template.PlanContent);
        
        }

        [Fact]
        public async Task GetTemplate_Null()
        {
            var versionId = Guid.NewGuid().ToString("D");
            var responseData = await _service.GetTemplateVersion(TemplateId, versionId, default);
            Assert.Null(responseData.Data);
            Assert.Equal(404, responseData.StatusCode);
            Assert.NotNull(responseData.Message);

        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
