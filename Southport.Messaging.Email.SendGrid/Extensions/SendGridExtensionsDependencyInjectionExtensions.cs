using Microsoft.Extensions.Configuration;
using Southport.Messaging.Email.SendGrid.Interfaces;
using Southport.Messaging.Email.SendGrid.Message;
using Southport.Messaging.Email.SendGrid.Message.Interfaces;
using Southport.Messaging.Email.SendGrid.Templates;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class SendGridExtensionsDependencyInjectionExtensions
{
    public static IServiceCollection AddSendGridServices(this IServiceCollection services, IConfigurationSection section)
    {
        services.Configure<SendGridOptions>(section);
        services.AddHttpClient<ISendGridMessageFactory, SendGridMessageFactory>();
        services.AddHttpClient<IDynamicTemplateService, DynamicTemplateService>();
        return services;
    }
    public static IServiceCollection AddSendGridServices(this IServiceCollection services, SendGridOptions options)
    {
        services.AddSingleton(Options.Options.Create(options));
        services.AddHttpClient<ISendGridMessageFactory, SendGridMessageFactory>();
        services.AddHttpClient<IDynamicTemplateService, DynamicTemplateService>();
        return services;
    }
}
