using Southport.Messaging.Email.Core;

namespace Southport.Messaging.Email.SendGrid.Interfaces
{
    public interface ISendGridMessageFactory : IEmailMessageFactory
    {
        new ISendGridMessage Create();


    }
}