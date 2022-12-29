using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HandlebarsDotNet;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using Southport.Messaging.Email.Core;
using Southport.Messaging.Email.Core.EmailAttachments;
using Southport.Messaging.Email.Core.Recipient;
using Southport.Messaging.Email.Core.Result;
using Southport.Messaging.Email.SendGrid.Extensions;
using Southport.Messaging.Email.SendGrid.Interfaces;
using EmailAddress = Southport.Messaging.Email.Core.Recipient.EmailAddress;

namespace Southport.Messaging.Email.SendGrid.Message
{
    public class SendGridMessage : ISendGridMessage
    {
        private readonly HttpClient _httpClient;
        private readonly ISendGridOptions _options;

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

        public ISendGridMessage SetFromAddress(IEmailAddress address)
        {
            FromAddress = address;
            return this;
        }

        public ISendGridMessage SetFromAddress(string address, string name = null)
        {
            FromAddress = new EmailAddress(address, name);
            return this;
        }

        #endregion

        #region ToAddresses
  
        public IEnumerable<IEmailRecipient> ToAddresses { get; set; } = new List<IEmailRecipient>();

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
            if (addresses == null || !addresses.Any())
            {
                return this;
            }
            ((List<IEmailRecipient>)ToAddresses).AddRange(addresses);
            return this;
        }

        #endregion

        #region CcAddresses

        private List<IEmailAddress> _ccAddresses = new();
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
            if (addresses == null || !addresses.Any())
            {
                return this;
            }
            _ccAddresses.AddRange(addresses);
            return this;
        }

        #endregion

        #region BccAddresses
        
        private List<IEmailAddress> _bccAddresses = new();
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
            if (addresses == null || !addresses.Any())
            {
                return this;
            }
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

        public List<IEmailAttachment> Attachments { get; set; } = new();
        
        public ISendGridMessage AddAttachments(IEmailAttachment attachment)
        {
            Attachments.Add(attachment);
            return this;
        }

        public ISendGridMessage AddAttachments(List<IEmailAttachment> attachments)
        {
            if (attachments == null || !attachments.Any())
            {
                return this;
            }
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

        public List<string> Categories { get; set; } = new();

        public ISendGridMessage SetCategory(string tag)
        {
            Categories.Add(tag);
            return this;
        }

        public ISendGridMessage SetCategories(List<string> categories)
        {
            if (categories == null || !categories.Any())
            {
                return this;
            }
            Categories.AddRange(categories);
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

        #region Batch

        public string BatchId{get; set;}

        public ISendGridMessage SetBatchId(string batchId)
        {
            BatchId = batchId;
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
        
        public Dictionary<string, string> CustomArguments { get; } = new ();

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

        #region Substitutions

        public Dictionary<string, object> Substitutions { get; } = new();

        public ISendGridMessage AddSubstitution(string key, object value)
        {
            Substitutions[key] = value;
            return this;
        }

        public ISendGridMessage AddSubstitutions(Dictionary<string, object> substitutions)
        {
            foreach (var substitution in substitutions)
            {
                Substitutions[substitution.Key] = substitution.Value;
            }

            return this;
        }

        #endregion

        #region Custom Headers

        public Dictionary<string, string> CustomHeaders { get; set; } = new();
        
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

        #region Core Methods

        IEmailMessageCore IEmailMessageCore.AddFromAddress(string emailAddress, string name)
        {
            return AddFromAddress(emailAddress, name);
        }

        IEmailMessageCore IEmailMessageCore.AddFromAddress(IEmailAddress emailAddress)
        {
            return AddFromAddress(emailAddress);
        }

        IEmailMessageCore IEmailMessageCore.SetFromAddress(string emailAddress, string name)
        {
            return AddFromAddress(emailAddress, name);
        }

        IEmailMessageCore IEmailMessageCore.SetFromAddress(IEmailAddress emailAddress)
        {
            return AddFromAddress(emailAddress);
        }

        IEmailMessageCore IEmailMessageCore.AddToAddress(IEmailRecipient recipient)
        {
            return AddToAddress(recipient);
        }

        IEmailMessageCore IEmailMessageCore.AddToAddress(string emailAddress, string name)
        {
            return AddToAddress(emailAddress, name);
        }

        IEmailMessageCore IEmailMessageCore.AddToAddresses(List<IEmailRecipient> recipients)
        {
            return AddToAddresses(recipients);
        }

        IEmailMessageCore IEmailMessageCore.AddCcAddress(IEmailAddress emailAddress)
        {
            return AddCcAddress(emailAddress);
        }

        IEmailMessageCore IEmailMessageCore.AddCcAddress(string emailAddress, string name)
        {
            return AddCcAddress(emailAddress, name);
        }

        IEmailMessageCore IEmailMessageCore.AddCcAddresses(List<IEmailAddress> emailAddresses)
        {
            return AddCcAddresses(emailAddresses);
        }

        IEmailMessageCore IEmailMessageCore.AddBccAddress(IEmailAddress emailAddress)
        {
            return AddBccAddress(emailAddress);
        }

        IEmailMessageCore IEmailMessageCore.AddBccAddress(string emailAddress, string name)
        {
            return AddBccAddress(emailAddress, name);
        }

        IEmailMessageCore IEmailMessageCore.AddBccAddresses(List<IEmailAddress> emailAddresses)
        {
            return AddBccAddresses(emailAddresses);
        }

        IEmailMessageCore IEmailMessageCore.SetSubject(string subject)
        {
            return SetSubject(subject);
        }

        IEmailMessageCore IEmailMessageCore.SetText(string text)
        {
            return SetText(text);
        }

        IEmailMessageCore IEmailMessageCore.SetHtml(string html)
        {
            return SetHtml(html);
        }

        IEmailMessageCore IEmailMessageCore.AddAttachments(IEmailAttachment attachment)
        {
            return AddAttachments(attachment);
        }

        IEmailMessageCore IEmailMessageCore.AddAttachments(List<IEmailAttachment> attachments)
        {
            return AddAttachments(attachments);
        }

        IEmailMessageCore IEmailMessageCore.SetTemplate(string template)
        {
            return SetTemplate(template);
        }

        IEmailMessageCore IEmailMessageCore.SetDeliveryTime(DateTime deliveryTime)
        {
            return SetDeliveryTime(deliveryTime);
        }

        IEmailMessageCore IEmailMessageCore.SetTestMode(bool testMode)
        {
            return SetTestMode(testMode);
        }

        IEmailMessageCore IEmailMessageCore.SetTracking(bool tracking)
        {
            return SetTracking(tracking);
        }

        IEmailMessageCore IEmailMessageCore.SetTrackingClicks(bool tracking)
        {
            return SetTrackingClicks(tracking);
        }

        IEmailMessageCore IEmailMessageCore.SetTrackingOpens(bool tracking)
        {
            return SetTrackingOpens(tracking);
        }

        IEmailMessageCore IEmailMessageCore.SetReplyTo(string emailAddress)
        {
            return SetReplyTo(emailAddress);
        }

        IEmailMessageCore IEmailMessageCore.AddCustomArgument(string key, string value)
        {
            return AddCustomArgument(key, value);
        }

        IEmailMessageCore IEmailMessageCore.AddCustomArguments(Dictionary<string, string> customArguments)
        {
            return AddCustomArguments(customArguments);
        }

        IEmailMessageCore IEmailMessageCore.AddSubstitution(string key, object value)
        {
            return AddSubstitution(key, value);
        }

        IEmailMessageCore IEmailMessageCore.AddSubstitutions(Dictionary<string, object> customArguments)
        {
            return AddSubstitutions(customArguments);
        }

        #endregion

        public SendGridMessage(HttpClient httpClient, ISendGridOptions options, bool tracking = true, bool trackingClicks = true, bool trackingOpens = true)
        {
            _httpClient = httpClient;
            
            _options = options;

            Tracking = tracking;
            TrackingClicks = trackingClicks;
            TrackingOpens = trackingOpens;

            TestMode = _options.UseTestMode;
        }

        #region Send

        public async Task<IEnumerable<IEmailResult>> Send(CancellationToken cancellationToken = default)
        {
            return await Send(false, false, cancellationToken);
        }

        public async Task<IEnumerable<IEmailResult>> Send(bool substitute, bool batch = false, CancellationToken cancellationToken = default)
        {
            if (FromAddress == null)
            {
                throw new SouthportMessagingException("The from address is required.");
            }

            if (ToAddressesValid.Any() == false && CcAddressesValid.Any() == false && BccAddressesValid.Any() == false)
            {
                throw new SouthportMessagingException("There must be at least 1 recipient.");
            }

            if (string.IsNullOrWhiteSpace(Html) && string.IsNullOrWhiteSpace(Text) && string.IsNullOrWhiteSpace(TemplateId))
            {
                throw new SouthportMessagingException("The message must have a message or reference a template.");
            }

            var sendGridApiMessages = GetMessageApi(substitute);

            if (batch)
            {
                BatchId = await GetBatchIdAsync(cancellationToken);
                if (BatchId == null)
                {
                    throw new Exception("Could not get a new Batch ID");
                }
            }

            var results = new List<IEmailResult>();
            foreach (var message in sendGridApiMessages)
            {
                var responseMessage = await SendAsync(message.Value, cancellationToken);
                var result = new EmailResult(message.Key, responseMessage.IsSuccessStatusCode, await responseMessage.Content.ReadAsStringAsync(cancellationToken));
                results.Add(result);
            }

            return results;
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

            var ccAddresses = CcAddressesSendGridValid.Any() ? CcAddressesSendGridValid.Where(e => e.Email != emailRecipient.EmailAddress.Address && BccAddressesSendGridValid.Any(bcc => bcc.Email == e.Email) == false).ToList() : null;
            var bccAddresses = BccAddressesSendGridValid.Any() ? BccAddressesSendGridValid.Where(e => e.Email != emailRecipient.EmailAddress.Address && CcAddressesSendGridValid.Any(bcc => bcc.Email == e.Email) == false).ToList() : null;

            var substitutions = emailRecipient.Substitutions ?? new Dictionary<string, object>();
            foreach (var substitution in Substitutions.Where(s => !substitutions.ContainsKey(s.Key)))
            {
                substitutions[substitution.Key] = substitution.Value;
            }

            var customArgs = emailRecipient.CustomArguments ?? new Dictionary<string, string>();
            foreach (var arg in CustomArguments.Where(a=>!customArgs.ContainsKey(a.Key)))
            {
                customArgs[arg.Key] = arg.Value;
            }

            message.AddTo(new global::SendGrid.Helpers.Mail.EmailAddress(emailRecipient.EmailAddress.Address, emailRecipient.EmailAddress.Name), personalization: new Personalization()
            {
                Ccs = ccAddresses != null && ccAddresses.Any() ? ccAddresses : null,
                Bccs = bccAddresses != null && bccAddresses.Any() ? bccAddresses : null,
                TemplateData = string.IsNullOrWhiteSpace(TemplateId) == false && substitutions.Any() ? substitutions : null,
                CustomArgs = customArgs.Any() ? customArgs : null
            });

            #region ReplyTo

            if (ReplyToAddress != null)
            {

                message.SetReplyTo(
                    new global::SendGrid.Helpers.Mail.EmailAddress(ReplyToAddress.Address, ReplyToAddress.Name));
            }

            #endregion

            #region From

            message.SetFrom(FromAddress.Address, FromAddress.Name);

            #endregion

            #region Subject
            
            message.Subject = Subject;

            #endregion

            #region Text/HTML/Template

            if (!string.IsNullOrWhiteSpace(TemplateId))
            {
                message.TemplateId = TemplateId;
                if (emailRecipient.Substitutions.ContainsKey("subject") == false)
                {
                    emailRecipient.Substitutions["subject"] = Subject;
                }
            }
            else
            {
                message.PlainTextContent = Substitute(Text, substitute ? substitutions : null);
                message.HtmlContent = Substitute(Html, substitute ? substitutions : null);
            }

            

            #endregion

            #region Categories

            message.Categories = Categories.Any() ? Categories : null;

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

            message.CustomArgs = CustomArguments.Any() ? CustomArguments : null;

            #endregion

            #region CustomHeaders

            message.Headers = CustomHeaders.Any() ? CustomHeaders : null;

            #endregion

            #region Attachments

            foreach (var attachment in Attachments)
            {
                message.AddAttachment(attachment.AttachmentFilename, attachment.Content, attachment.AttachmentType);
            }

            #endregion

            return message;

        }

        private static string Substitute(string text, Dictionary<string, object> substitutions)
        {
            if (string.IsNullOrEmpty(text)) return "";
            if (substitutions == null || !substitutions.Any()) return text;

            var compileFunc = Handlebars.Compile(text);
            text = compileFunc(substitutions);

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
                CcAddresses = new List<IEmailAddress>();
            }

            if (BccAddresses.Any())
            {
                BccAddresses = new List<IEmailAddress>();
            }

            toAddresses = toAddressesTemp;

            return toAddresses;
        }

        #endregion

        #region HTTP

        private async Task<HttpResponseMessage> SendAsync(global::SendGrid.Helpers.Mail.SendGridMessage message, CancellationToken cancellationToken)
        {
            var json = message.Serialize();
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync("mail/send", stringContent, cancellationToken);
        }

        private async Task<string> GetBatchIdAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsync("mail/batch", null, cancellationToken);
            if (!response.IsSuccessStatusCode) return null;

            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
            var batch = JsonConvert.DeserializeObject<SendGridBatchResponse>(responseString);
            return batch.BatchId;

        }

        #endregion
    }
}