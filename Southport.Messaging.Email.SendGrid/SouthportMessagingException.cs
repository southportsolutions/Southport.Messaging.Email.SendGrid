using System;

namespace Southport.Messaging.Email.SendGrid
{
    public class SouthportMessagingException : Exception
    {
        public SouthportMessagingException(string message) : base(message){}
    }
}
