using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Southport.Messaging.Email.SendGrid.HttpClients
{
    public interface ISendGridHttpClient
    {
        Task<HttpResponseMessage> SendAsync(global::SendGrid.Helpers.Mail.SendGridMessage message, CancellationToken cancellationToken);
        Task<string> GetBatchIdAsync(CancellationToken cancellationToken);
    }
}