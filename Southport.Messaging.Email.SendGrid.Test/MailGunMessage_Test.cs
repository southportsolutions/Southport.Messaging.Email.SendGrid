using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Southport.Messaging.Email.Core.EmailAttachments;
using Southport.Messaging.Email.Core.Recipient;
using Southport.Messaging.Email.SendGrid.HttpClients;
using Southport.Messaging.Email.SendGrid.Interfaces;
using Southport.Messaging.Email.SendGrid.Message;
using Xunit;
using Xunit.Abstractions;

namespace Southport.Messaging.Email.SendGrid.Test
{
    public class SendGridMessageTest : IDisposable
    {
        private const string SubjectPrefix = "SendGrid - ";
        private const string TemplateId = "d-63308b30a4234c7b8fc2050926526538";
        private readonly SendGridHttpClient _httpClient;
        private readonly HttpClient _internalHttpClient;
        private readonly ISendGridOptions _options;
        
        private readonly  ITestOutputHelper _output;

        public SendGridMessageTest(ITestOutputHelper output)
        {
            _output = output;
            _options = Startup.GetOptions();
            _internalHttpClient = new HttpClient();
            _httpClient = new SendGridHttpClient(_internalHttpClient, _options);
        }

        #region Simple Message

        [Fact]
        public async Task Send_Simple_Message()
        {
            var emailAddress = "test1@southport.solutions";
            var message = new SendGridMessage(_httpClient, _options);
            var responses = await message.AddFromAddress("test2@southport.solutions")
                .AddToAddress(emailAddress)
                .SetSubject($"{SubjectPrefix}Simple")
                .SetText("This is a test email.").Send();
            

            foreach (var response in responses)
            {
                _output.WriteLine(await response.ResponseMessage.Content.ReadAsStringAsync());
                Assert.True(response.ResponseMessage.IsSuccessStatusCode);
                Assert.Equal(emailAddress, response.EmailRecipient.EmailAddress.Address);
            }
        }

        [Fact]
        public async Task SetReplyTo()
        {

            var handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(handlerMock.Object);

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            };

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var sendGridClient = new SendGridHttpClient(httpClient, _options);

            var emailAddress = "michael@southportsolutions.com";
            var replyTo = "test2@southport.solutions";
            var replyToName = "name";
            var message = new SendGridMessage(sendGridClient, _options);
            var responses = await message.AddFromAddress("test2@southport.solutions")
                .AddToAddress(emailAddress)
                .SetSubject($"{SubjectPrefix}Simple")
                .SetReplyTo(new EmailAddress(replyTo, replyToName))
                .SetText("This is a test email.").Send();


            var request = (HttpRequestMessage) handlerMock.Invocations[0].Arguments[0];

            var str = await request.Content.ReadAsStringAsync();

            var content = JsonConvert.DeserializeObject<dynamic>(str);

            var replyToResult = (string) content.reply_to.email;
            var replyToNameResult = (string)content.reply_to.name;

            Assert.Equal(replyTo, replyToResult);
            Assert.Equal(replyToName, replyToNameResult);
        }

        [Fact]
        public async Task Send_Batch_Message()
        {
            var emailAddress = "test1@southport.solutions";
            var message = new SendGridMessage(_httpClient, _options);
            var responses = await message.AddFromAddress("test2@southport.solutions")
                .AddToAddress(emailAddress)
                .SetSubject($"{SubjectPrefix}Simple")
                .SetDeliveryTime(DateTime.UtcNow.AddSeconds(20))
                .SetText("This is a test email.").Send(false, true, CancellationToken.None);
            

            foreach (var response in responses)
            {
                _output.WriteLine(await response.ResponseMessage.Content.ReadAsStringAsync());
                Assert.True(response.ResponseMessage.IsSuccessStatusCode);
                Assert.Equal(emailAddress, response.EmailRecipient.EmailAddress.Address);
            }
        }

        [Fact]
        public async Task Send_Simple_Message_Multiple_Same_Addresses()
        {
            var emailAddress = "test1@southport.solutions";
            var message = new SendGridMessage(_httpClient, _options);
            var responses = await message.AddFromAddress("test2@southport.solutions")
                .AddToAddress(emailAddress)
                .AddCcAddress("test1@southport.solutions")
                .AddCcAddress("test2@southport.solutions")
                .AddBccAddress("test2@southport.solutions")
                .AddBccAddress("test3@southport.solutions")
                .SetSubject($"{SubjectPrefix}Simple")
                .SetText("This is a test email.").Send();
            

            foreach (var response in responses)
            {
                _output.WriteLine(await response.ResponseMessage.Content.ReadAsStringAsync());
                Assert.True(response.ResponseMessage.IsSuccessStatusCode);
                Assert.Equal(emailAddress, response.EmailRecipient.EmailAddress.Address);
            }
        }

        [Fact]
        public async Task Send_Simple_Attachment_Message()
        {
            var emailAddress = "test1@southport.solutions";
            var message = new SendGridMessage(_httpClient, _options);
            var responses = await message.AddFromAddress("test2@southport.solutions")
                .AddToAddress(emailAddress)
                .SetSubject($"{SubjectPrefix}Simple with Attachment")
                .AddAttachments(new EmailAttachment()
                {
                    AttachmentFilename = "test.txt",
                    AttachmentType = "text/plain", 
                    Content = "Test attachment content."
                })
                .SetText("This is a test email.").Send();
            

            foreach (var response in responses)
            {
                _output.WriteLine(await response.ResponseMessage.Content.ReadAsStringAsync());
                Assert.True(response.ResponseMessage.IsSuccessStatusCode);
                Assert.Equal(emailAddress, response.EmailRecipient.EmailAddress.Address);
            }
        }

        #endregion

        #region Message With Sutstituions

        [Fact]
        public async Task Send_Message_Text_WithSubstitutions()
        {
            var emailAddress = new EmailRecipient("test1@southport.solutions", substitutions: new Dictionary<string, object>() {["FirstName"] = "Robert"});
            var message = new SendGridMessage(_httpClient, _options);
            var responses = await message.AddFromAddress("test2@southport.solutions")
                .AddToAddress(emailAddress)
                .SetSubject($"{SubjectPrefix}Text with Substitutions")
                .SetText("Dear {{FirstName}} This is a test email.").Send(true);


            foreach (var response in responses)
            {
                _output.WriteLine(await response.ResponseMessage.Content.ReadAsStringAsync());
                Assert.True(response.ResponseMessage.IsSuccessStatusCode);
                Assert.Equal(emailAddress.EmailAddress.Address, response.EmailRecipient.EmailAddress.Address);
            }
        }

        [Fact]
        public async Task Send_Message_Html_WithSubstitutions()
        {
            var html = await File.ReadAllTextAsync(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Templates/Html.html"));
            var emailRecipients = new List<IEmailRecipient>()
            {
                new EmailRecipient("test1@southport.solutions", substitutions: new Dictionary<string, object>() {["FirstName"] = "Robert"}),
                new EmailRecipient("test1@southport.solutions", substitutions: new Dictionary<string, object>() {["FirstName"] = "David"})
            };

            var message = new SendGridMessage(_httpClient, _options);
            var responses = (await message.AddFromAddress("test2@southport.solutions")
                .AddToAddresses(emailRecipients)
                .SetSubject($"{SubjectPrefix}Html With Substitutions")
                .SetHtml(html).Send(true)).ToList();


            for (var i = 0; i < responses.Count(); i++)
            {
                var response = responses.ElementAt(i);
                var recipient = emailRecipients.ElementAt(i);
                _output.WriteLine(await response.ResponseMessage.Content.ReadAsStringAsync());
                Assert.True(response.ResponseMessage.IsSuccessStatusCode);
                Assert.Equal(recipient.EmailAddress.Address, response.EmailRecipient.EmailAddress.Address);
            }
        }

        #endregion

        #region Template

        [Fact]
        public async Task Send_Template_Message()
        {
            var recipient = new EmailRecipient("test1@southport.solutions", substitutions: new Dictionary<string, object>()
            {
                {"name", "John Doe"},
                {
                    "states", new List<string>
                    {
                        "CA",
                        "CT",
                        "TN"
                    }
                }
            });
            var message = new SendGridMessage(_httpClient, _options);
            var responses = await message.AddFromAddress("test2@southport.solutions")
                .AddToAddress(recipient)
                .SetSubject($"{SubjectPrefix}Template")
                .SetTemplate(TemplateId).Send();

            foreach (var response in responses)
            {
                _output.WriteLine(await response.ResponseMessage.Content.ReadAsStringAsync());
                Assert.True(response.ResponseMessage.IsSuccessStatusCode);
                Assert.Equal(recipient.EmailAddress.Address, response.EmailRecipient.EmailAddress.Address);
            }
        }

        [Fact]
        public async Task Send_Template_TestEmailAddress_Message()
        {
            var testAddresses = new List<string>() {"test2@southport.solutions", "test3@southport.solutions"};

            var options = new EmailOptions()
            {
                ApiKey = _options.ApiKey,
                TestEmailAddresses = string.Join(",", testAddresses),
                UseTestMode = _options.UseTestMode
            };
            var recipient = new EmailRecipient("rob@southportsolutions.com", substitutions: new Dictionary<string, object>() {{"name", "John Doe"}, {"states", new List<string> {"CA", "CT", "TN"}}});
            var message = new SendGridMessage(_httpClient, options);
            var responses = await message.AddFromAddress("test1@southport.solutions")
                .AddToAddress(recipient)
                .SetSubject($"{SubjectPrefix}Test Email Addresses")
                .SetTemplate(TemplateId).Send();

            foreach (var response in responses)
            {
                _output.WriteLine(await response.ResponseMessage.Content.ReadAsStringAsync());
                Assert.True(response.ResponseMessage.IsSuccessStatusCode);
                Assert.Contains(testAddresses, s => s.Equals(response.EmailRecipient.EmailAddress.Address));
            }
        }

        [Fact]
        public async Task Send_Template_WithAttachment_TestEmailAddress_Message()
        {
            var testAddresses = new List<string>() {"test2@southport.solutions", "test3@southport.solutions"};

            var options = new EmailOptions()
            {
                ApiKey = _options.ApiKey,
                TestEmailAddresses = string.Join(",", testAddresses),
                UseTestMode = _options.UseTestMode
            };
            
            var timeZone = new VTimeZone("America/Chicago");
            var calendarEvent = new CalendarEvent
            {
                Start = new CalDateTime(DateTime.UtcNow.AddDays(1), timeZone.Location),
                End = new CalDateTime(DateTime.UtcNow.AddDays(1).AddHours(1), timeZone.Location),
                Description = "Test Event",
                Location = "Test Location",
                Summary = "Test Summary",
            };


            var calendar = new Calendar();

            calendar.Events.Add(calendarEvent);

            var serializer = new CalendarSerializer();
            var icalString = serializer.SerializeToString(calendar);

            var recipient = new EmailRecipient("to@test.com", substitutions: new Dictionary<string, object>() {{"name", "John Doe"}, {"states", new List<string> {"CA", "CT", "TN"}}});
            recipient.Attachments.Add(new EmailAttachment()
            {
                AttachmentFilename = "calendar.ics",
                AttachmentType = "text/calendar",
                Content = icalString
            });

            var message = new SendGridMessage(_httpClient, options);
            var responses = await message.AddFromAddress("test1@southport.solutions")
                .AddToAddress(recipient)
                .AddCcAddress("cc@test.com")
                .AddBccAddress("bcc@test.com")
                .SetSubject($"{SubjectPrefix}Test Email Addresses With Attachment")
                .SetTemplate(TemplateId).Send();

            foreach (var response in responses)
            {
                _output.WriteLine(await response.ResponseMessage.Content.ReadAsStringAsync());
                Assert.True(response.ResponseMessage.IsSuccessStatusCode);
                Assert.Contains(testAddresses, s => s.Equals(response.EmailRecipient.EmailAddress.Address));
            }
        }

        #endregion

        public void Dispose()
        {
            _internalHttpClient.Dispose();
        }
    }
}
