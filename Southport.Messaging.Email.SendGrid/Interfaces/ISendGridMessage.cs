using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Southport.Messaging.Email.Core;
using Southport.Messaging.Email.Core.Recipient;
using Southport.Messaging.Email.Core.Result;

namespace Southport.Messaging.Email.SendGrid.Interfaces
{
    public interface ISendGridMessage : IEmailMessage<ISendGridMessage>
    {
        IEmailAddress ReplyToAddress { get; set; }
        List<string> Categories { get; set; }

        ISendGridMessage SetReplyTo(IEmailAddress emailAddress);
        ISendGridMessage SetCategory(string tag);
        ISendGridMessage SetCategories(List<string> tags);
        ISendGridMessage AddHeader(string key, string header);

        Task<IEnumerable<IEmailResult>> SubstituteAndSend(CancellationToken cancellationToken = default);

    }
}