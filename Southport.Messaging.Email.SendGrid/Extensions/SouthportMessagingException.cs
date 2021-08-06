using System;

namespace Southport.Messaging.Email.SendGrid.Extensions
{
    public class SouthportMessagingException : Exception
    {
        public SouthportMessagingException(string message) : base(message){}
    }
}
