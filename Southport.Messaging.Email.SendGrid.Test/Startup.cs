using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Southport.Messaging.Email.SendGrid.Interfaces;

namespace Southport.Messaging.Email.SendGrid.Test
{
    public static class Startup
    {
        public static SendGridOptions Options { get; private set; }

        public static SendGridOptions GetOptions()
        {
            if (Options == null)
            {
                var configurationBuilder = new ConfigurationBuilder()
                    
                    .AddJsonFile(Path.Combine((new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent).ToString(), "appsettings.json"), true)
                    .AddEnvironmentVariables();
                var config = configurationBuilder.Build();
                Options = new SendGridOptions();
                var section = config.GetSection("SendGrid");
                section.Bind(Options);

                if (string.IsNullOrWhiteSpace(Options.ApiKey))
                {
                    Options.ApiKey = Environment.GetEnvironmentVariable("APIKEY");
                    Options.UseTestMode = bool.Parse(Environment.GetEnvironmentVariable("USETESTMODE") ?? "false");
                }

                if (string.IsNullOrEmpty(Options.ApiKey))
                {
                    throw new Exception("Unable to get the Sendgrid API Key.");
                }
            }

            return Options;

        }
    }
}
