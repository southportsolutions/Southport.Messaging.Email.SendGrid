using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HandlebarsDotNet;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using Southport.Messaging.Email.Core.EmailAttachments;
using Southport.Messaging.Email.Core.Recipient;
using Southport.Messaging.Email.Core.Result;
using Southport.Messaging.Email.SendGrid.Interfaces;
using EmailAddress = Southport.Messaging.Email.Core.Recipient.EmailAddress;

namespace Southport.Messaging.Email.SendGrid
{
    public class SendGridMessage : ISendGridMessage
    {
        private readonly HttpClient _httpClient;
        private readonly ISendGridOptions _options;
        private List<Stream> _streams = new List<Stream>();

        #region FromAddress
        
        public IEmailAddress FromAddress { get; set; }

        public string From => FromAddress.ToString();

        public ISendGridMessage AddFromAddress(IEmailAddress address)
        {
            FromAddress = address;
            return this;
        }

        public ISendGridMessage AddFromAddress(string address, string name = null)
        {
            FromAddress = new EmailAddress(address, name);
            return this;
        }

        #endregion

        #region ToAddresses
  
        public IEnumerable<IEmailRecipient> ToAddresses { get; set; }

        public IEnumerable<IEmailRecipient> ToAddressesValid => ToAddresses.Where(e => e.EmailAddress.IsValid);
        public IEnumerable<IEmailRecipient> ToAddressesInvalid => ToAddresses.Where(e => e.EmailAddress.IsValid==false);

        //private string To => string.Join(";", ToAddresses);

        public ISendGridMessage AddToAddress(IEmailRecipient address)
        {
            ((List<IEmailRecipient>)ToAddresses).Add(address);
            return this;
        }

        public ISendGridMessage AddToAddress(string address, string name = null)
        {
            return AddToAddress(new EmailRecipient(address, name));
        }

        public ISendGridMessage AddToAddresses(List<IEmailRecipient> addresses)
        {
            ((List<IEmailRecipient>)ToAddresses).AddRange(addresses);
            return this;
        }

        #endregion

        #region CcAddresses

        private List<IEmailAddress> _ccAddresses = new List<IEmailAddress>();
        public IEnumerable<IEmailAddress> CcAddresses
        {
            get => _ccAddresses;
            set => _ccAddresses = value.ToList();
        }

        private List<global::SendGrid.Helpers.Mail.EmailAddress> CcAddressesSendGridValid => CcAddressesValid.Select(c => new global::SendGrid.Helpers.Mail.EmailAddress(c.Address, c.Name)).ToList();

        public IEnumerable<IEmailAddress> CcAddressesValid => CcAddresses.Where(e => e.IsValid);
        public IEnumerable<IEmailAddress> CcAddressesInvalid =>  CcAddresses.Where(e => e.IsValid==false);
        //private string Cc => string.Join(";", CcAddresses);

        public ISendGridMessage AddCcAddress(IEmailAddress address)
        {
            _ccAddresses.Add(address);
            return this;
        }

        public ISendGridMessage AddCcAddress(string address, string name = null)
        {
            return AddCcAddress(new EmailAddress(address, name));
        }

        public ISendGridMessage AddCcAddresses(List<IEmailAddress> addresses)
        {
            _ccAddresses.AddRange(addresses);
            return this;
        }

        #endregion

        #region BccAddresses
        
        private List<IEmailAddress> _bccAddresses = new List<IEmailAddress>();
        public IEnumerable<IEmailAddress> BccAddresses
        {
            get => _bccAddresses;
            set => _bccAddresses = value.ToList();
        }

        private List<global::SendGrid.Helpers.Mail.EmailAddress> BccAddressesSendGridValid => BccAddressesValid.Select(c => new global::SendGrid.Helpers.Mail.EmailAddress(c.Address, c.Name)).ToList();

        public IEnumerable<IEmailAddress> BccAddressesValid => BccAddresses.Where(e => e.IsValid);
        public IEnumerable<IEmailAddress> BccAddressesInvalid => BccAddresses.Where(e => e.IsValid==false);
        //private string Bcc => string.Join(";", BccAddresses);
        
        public ISendGridMessage AddBccAddress(IEmailAddress address)
        {
            _bccAddresses.Add(address);
            return this;
        }

        public ISendGridMessage AddBccAddress(string address, string name = null)
        {
            return AddBccAddress(new EmailAddress(address, name));
        }

        public ISendGridMessage AddBccAddresses(List<IEmailAddress> addresses)
        {
            _bccAddresses.AddRange(addresses);
            return this;
        }

        #endregion

        #region ReplyTo

        public IEmailAddress ReplyToAddress { get; set; }
        public ISendGridMessage SetReplyTo(IEmailAddress emailAddress)
        {
            ReplyToAddress = emailAddress;
            return this;
        }

        public ISendGridMessage SetReplyTo(string emailAddress)
        {
            ReplyToAddress = new EmailAddress(emailAddress);
            return this;
        }

        #endregion

        #region Subject

        public string Subject { get; private set; }
        
        public ISendGridMessage SetSubject(string subject)
        {
            Subject = subject;
            return this;
        }

        #endregion

        #region Text

        public string Text { get; set; }
        
        public ISendGridMessage SetText(string text)
        {
            Text = text.Trim();
            return this;
        }

        #endregion

        #region HTML

        public string Html { get; set; }
        
        public ISendGridMessage SetHtml(string html)
        {
            Html = html.Trim();
            return this;
        }

        #endregion

        #region Attachments

        public List<IEmailAttachment> Attachments { get; set; }
        
        public ISendGridMessage AddAttachments(IEmailAttachment attachment)
        {
            Attachments.Add(attachment);
            return this;
        }

        public ISendGridMessage AddAttachments(List<IEmailAttachment> attachments)
        {
            Attachments = attachments;
            return this;
        }

        #endregion

        #region Template

        public string TemplateId { get; set; }
        
        public ISendGridMessage SetTemplate(string template)
        {
            TemplateId = template;
            return this;
        }

        #endregion

        #region Categories
        
        public List<string> Categories { get; set; }

        public ISendGridMessage SetCategory(string tag)
        {
            Categories.Add(tag);
            return this;
        }

        public ISendGridMessage SetCategories(List<string> tags)
        {
            Categories.AddRange(tags);
            return this;
        }

        #endregion

        #region DeliveryTime
        
        public DateTime? DeliveryTime { get; set; }
        
        public ISendGridMessage SetDeliveryTime(DateTime deliveryTime)
        {
            DeliveryTime = deliveryTime;
            return this;
        }

        #endregion

        #region TestMode
        
        public bool? TestMode { get; set; }

        public ISendGridMessage SetTestMode(bool testMode)
        {
            TestMode = testMode;
            return this;
        }

        #endregion

        #region Tracking
        
        public bool Tracking { get; set; }

        public ISendGridMessage SetTracking(bool tracking)
        {
            Tracking = tracking;
            return this;
        }

        #endregion

        #region TrackingClicks
        
        public bool TrackingClicks { get; set; }
        
        public ISendGridMessage SetTrackingClicks(bool tracking)
        {
            TrackingClicks = tracking;
            return this;
        }

        #endregion

        #region TrackingOpens
        
        public bool TrackingOpens { get; set; }

        public ISendGridMessage SetTrackingOpens(bool tracking)
        {
            TrackingOpens = tracking;
            return this;
        }



        #endregion

        #region Custom Variables
        
        public Dictionary<string, string> CustomArguments { get; }

        public ISendGridMessage AddCustomArgument(string key, string value)
        {
            CustomArguments[key] = value;
            return this;
        }

        public ISendGridMessage AddCustomArguments(Dictionary<string, string> customArguments)
        {
            foreach (var argument in customArguments)
            {
                CustomArguments[argument.Key] = argument.Value;
            }

            return this;
        }

        #endregion

        #region Custom Headers
        
        public Dictionary<string, string> CustomHeaders { get; set; }
        
        public ISendGridMessage AddHeader(string key, string header)
        {
            CustomHeaders.Add(key, header);
            return this;
        }

        public SendGridMessage AddRecipientVariable(string emailAddress, string key, string value)
        {
            throw new NotImplementedException();
        }

        #endregion

        public SendGridMessage(HttpClient httpClient, ISendGridOptions options, bool tracking = true, bool trackingClicks = true, bool trackingOpens = true)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);

            _options = options;
            ToAddresses = new List<IEmailRecipient>();
            CcAddresses = new List<IEmailAddress>();
            BccAddresses = new List<IEmailAddress>();
            Attachments = new List<IEmailAttachment>();
            CustomHeaders = new Dictionary<string, string>();
            CustomArguments = new Dictionary<string, string>();
            Categories = new List<string>();


            Tracking = tracking;
            TrackingClicks = trackingClicks;
            TrackingOpens = trackingOpens;
        }

        #region Send

        public async Task<IEnumerable<IEmailResult>> Send(CancellationToken cancellationToken = default)
        {
            return await Send(_options.Domain, cancellationToken);
        }

        public async Task<IEnumerable<IEmailResult>> Send(string domain, CancellationToken cancellationToken = default)
        {
            return await Send(domain, false, cancellationToken);
        }

        private async Task<IEnumerable<IEmailResult>> Send(string domain, bool substitute = false, CancellationToken cancellationToken = default)
        {
            if (FromAddress == null)
            {
                throw new SouthportMessagingException("The from address is required.");
            }

            if (ToAddressesValid.Any()==false && CcAddressesValid.Any()==false && BccAddressesValid.Any()==false)
            {
                throw new SouthportMessagingException("There must be at least 1 recipient.");
            }

            if (string.IsNullOrWhiteSpace(Html) && string.IsNullOrWhiteSpace(Text) && string.IsNullOrWhiteSpace(TemplateId))
            {
                throw new SouthportMessagingException("The message must have a message or reference a template.");
            }

            var sendGridApiMessages = GetMessageApi();

            var results = new List<IEmailResult>();
            try
            {
                foreach (var message in sendGridApiMessages)
                {
                    var stringContent = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
                    var responseMessage = await _httpClient.PostAsync("https://api.sendgrid.com/v3/mail/send", stringContent, cancellationToken);
                    results.Add(new EmailResult(message.Key, responseMessage));
                }
            }
            finally
            {
                foreach (var stream in _streams)
                {
#if NET5_0 || NETSTANDARD2_1
                    await stream.DisposeAsync();
#else
                    stream.Dispose();
#endif
                }
            }
            
            return results;
        }

        public async Task<IEnumerable<IEmailResult>> SubstituteAndSend(CancellationToken cancellationToken = default)
        {
            return await Send(_options.Domain, true, cancellationToken);
        }
        
        public async Task<IEnumerable<IEmailResult>> SubstituteAndSend(string domain, CancellationToken cancellationToken = default)
        {
            return await Send(domain, true, cancellationToken);
        }

        #endregion

        #region Helpers

        private Dictionary<IEmailRecipient, global::SendGrid.Helpers.Mail.SendGridMessage> GetMessageApi(bool substitute = false)
        {
            var contents = new Dictionary<IEmailRecipient, global::SendGrid.Helpers.Mail.SendGridMessage>();
            var toAddresses = GetTestAddresses(ToAddressesValid.ToList());

            if (string.IsNullOrWhiteSpace(_options.TestEmailAddresses)==false)
            {
            }

            foreach (var emailRecipient in toAddresses)
            {
                contents[emailRecipient] = GetMessageApi(emailRecipient, substitute);
            }

            return contents;
        }
        
        private global::SendGrid.Helpers.Mail.SendGridMessage GetMessageApi(IEmailRecipient emailRecipient, bool substitute = false)
        {
            var message = new global::SendGrid.Helpers.Mail.SendGridMessage();

            message.AddTo(new global::SendGrid.Helpers.Mail.EmailAddress(emailRecipient.EmailAddress.Address, emailRecipient.EmailAddress.Name), personalization: new Personalization()
            {
                Ccs = CcAddressesSendGridValid,
                Bccs = BccAddressesSendGridValid,
                TemplateData = emailRecipient.Substitutions,
                CustomArgs = emailRecipient.CustomArguments
            });

            #region Subject
            
            message.Subject = Subject;

            #endregion

            #region Text/HTML/Template

            if (string.IsNullOrWhiteSpace(TemplateId))
            {
                Substitute(Text, "text", substitute ? emailRecipient.Substitutions : null);
                Substitute(Html, "html", substitute ? emailRecipient.Substitutions : null);
            }
            else
            {
                message.TemplateId = TemplateId;
            }

            #endregion

            #region Categories

            message.Categories = Categories;

            #endregion

            #region DeliveryTime

            if (DeliveryTime != null)
            {
                var seconds = (DeliveryTime.Value - new DateTime(1970, 1, 1)).TotalSeconds;
                message.SendAt = (long)Math.Floor(seconds);
            }

            #endregion

            #region TestMode

            if (TestMode == true)
            {
                message.SetSandBoxMode(true);
            }

            #endregion

            #region TrackingMode

            message.TrackingSettings = new TrackingSettings() {ClickTracking = new ClickTracking() {Enable = TrackingClicks}, OpenTracking = new OpenTracking() {Enable = TrackingOpens}};

            #endregion

            #region CustomArgs

            message.CustomArgs = CustomArguments;

            #endregion

            #region CustomHeaders

            message.Headers = CustomHeaders;

            #endregion

            #region Attachments

            foreach (var attachment in Attachments)
            {
                message.AddAttachment(attachment.AttachmentFilename, attachment.Content, attachment.AttachmentType);
            }

            #endregion

            return message;

        }

        private string Substitute(string text, string key, Dictionary<string, object> substitutions)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            if (substitutions != null && substitutions.Any())
            {
                var compileFunc = Handlebars.Compile(text);
                text = compileFunc(substitutions);
            }

            return text;
        }

        private IEnumerable<IEmailRecipient> GetTestAddresses(IEnumerable<IEmailRecipient> toAddresses)
        {
            if (string.IsNullOrWhiteSpace(_options.TestEmailAddresses))
            {
                return toAddresses;
            }

            var testEmailAddresses = _options.TestEmailAddresses.Split(',');
            var toAddressesTemp = new List<IEmailRecipient>();
            foreach (var toAddress in toAddresses)
            {
                var customArgs = toAddress.CustomArguments;
                customArgs["IsTest"] = "true";

                toAddressesTemp.AddRange(testEmailAddresses.Select(emailAddress => new EmailRecipient(emailAddress.Trim(), substitutions: toAddress.Substitutions,  customArguments: toAddress.CustomArguments, attachments: toAddress.Attachments)));
            }

            if (CcAddresses.Any())
            {
                CcAddresses = testEmailAddresses.Select(emailAddress => new EmailAddress(emailAddress.Trim()));
            }

            if (BccAddresses.Any())
            {
                BccAddresses = testEmailAddresses.Select(emailAddress => new EmailAddress(emailAddress.Trim()));
            }

            toAddresses = toAddressesTemp;

            return toAddresses;
        }

        private Stream GetStream(string content)
        {
            var stream = new MemoryStream();
            var sw = new StreamWriter(stream, Encoding.UTF8);
            sw.Write(content);
            sw.Flush();//otherwise you are risking empty stream
            stream.Seek(0, SeekOrigin.Begin);
            _streams.Add(stream);
            return stream;
        }

        #endregion
    }
}