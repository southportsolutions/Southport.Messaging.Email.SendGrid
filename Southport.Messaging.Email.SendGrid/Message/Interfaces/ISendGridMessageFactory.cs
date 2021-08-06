using Southport.Messaging.Email.Core;
using Southport.Messaging.Email.SendGrid.Interfaces;

namespace Southport.Messaging.Email.SendGrid.Message.Interfaces
{
    public interface ISendGridMessageFactory : IEmailMessageFactory
    {
        new ISendGridMessage Create();


    }
}