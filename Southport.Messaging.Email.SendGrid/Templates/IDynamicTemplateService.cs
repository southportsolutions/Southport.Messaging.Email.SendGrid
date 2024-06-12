using System.Threading;
using System.Threading.Tasks;
using Southport.Messaging.Email.SendGrid.Templates.Models;

namespace Southport.Messaging.Email.SendGrid.Templates;

public interface IDynamicTemplateService
{
    Task<ResponseData<DynamicTemplateVersion>> GetTemplateVersion(string templateId, string versionId, CancellationToken cancellationToken);
}