# Southport.Messaging.Email.SendGrid

[![NuGet](https://img.shields.io/nuget/v/Southport.Messaging.Email.SendGrid.svg)](https://www.nuget.org/packages/Southport.Messaging.Email.SendGrid/)

SendGrid implementation of [Southport.Messaging.Email.Core](https://www.nuget.org/packages/Southport.Messaging.Email.Core).
It provides a fluent message builder that sends one SendGrid API request per recipient, with
per-recipient substitutions, custom arguments, attachments, dynamic templates, categories,
batch IDs, scheduled delivery, and tracking settings.

Targets `net9.0` and `net10.0`. Licensed under MIT.

## Installation

Available on [NuGet](https://www.nuget.org/packages/Southport.Messaging.Email.SendGrid/).

```shell
dotnet add package Southport.Messaging.Email.SendGrid
```

## Configuration

Add a `SendGrid` section to `appsettings.json`:

```json
{
  "SendGrid": {
    "ApiKey": "SG.xxxxxxxx",
    "UseTestMode": false,
    "TestEmailAddresses": ""
  }
}
```

| Option | Description |
| --- | --- |
| `ApiKey` | SendGrid API key with mail send permission. Required. |
| `UseTestMode` | When `true`, every message is sent with SendGrid sandbox mode enabled. SendGrid validates the request but does not deliver it. Can be overridden per message with `SetTestMode`. |
| `TestEmailAddresses` | Comma separated list. When set, every message is delivered to these addresses instead of the real recipients. CC and BCC addresses are dropped and a custom argument `IsTest=true` is added. |

Register the services:

```csharp
builder.Services.AddSendGridServices(builder.Configuration.GetSection("SendGrid"));
```

Or pass the options directly:

```csharp
builder.Services.AddSendGridServices(new SendGridOptions
{
    ApiKey = "SG.xxxxxxxx",
    UseTestMode = true
});
```

Both overloads register `ISendGridMessageFactory` and `IDynamicTemplateService` as typed HTTP clients.

## Sending a message

Inject `ISendGridMessageFactory` and build a message from it. Each message is disposable and
should be used once.

```csharp
public class WelcomeMailer(ISendGridMessageFactory factory)
{
    public async Task SendAsync(string toAddress, CancellationToken cancellationToken)
    {
        await using var message = factory.Create();

        var results = await message
            .SetFromAddress("no-reply@example.com", "Example")
            .AddToAddress(toAddress)
            .SetSubject("Welcome")
            .SetHtml("<p>Thanks for signing up.</p>")
            .SetText("Thanks for signing up.")
            .Send(cancellationToken);

        foreach (var result in results)
        {
            if (result.IsSuccessful == false)
            {
                // result.Message contains the SendGrid response body.
            }
        }
    }
}
```

`Send` returns one `IEmailResult` per recipient. A `SouthportMessagingException` is thrown
before any request is made when the from address is missing, there are no valid recipients,
or the message has no text, HTML, or template.

### Recipients

`AddToAddress`, `AddCcAddress`, and `AddBccAddress` accept a plain address or a typed
`EmailRecipient` / `EmailAddress`. Addresses that fail validation are kept on the message but
are not sent to. Each recipient gets its own API request. CC and BCC addresses are attached to
every recipient's request, except where the address matches the recipient or appears in both
lists.

### Substitutions and custom arguments

Message-level substitutions apply to every recipient. Recipient-level substitutions override
them.

```csharp
await using var message = factory.Create();
var results = await message
    .SetFromAddress("no-reply@example.com")
    .AddToAddress(new EmailRecipient("alice@example.com",
        substitutions: new Dictionary<string, object> { ["FirstName"] = "Alice" }))
    .AddSubstitution("Company", "Example")
    .SetSubject("Hello")
    .SetText("Hi {{FirstName}}, welcome to {{Company}}.")
    .Send(substitute: true);
```

With `substitute: true`, text and HTML bodies are rendered with Handlebars before sending.
With a template ID set, substitutions are sent as dynamic template data instead and SendGrid
renders them.

`AddCustomArgument` attaches key/value pairs to the message for use in SendGrid event webhooks.
Recipient-level custom arguments override message-level ones.

### Dynamic templates

```csharp
await message
    .SetFromAddress("no-reply@example.com")
    .AddToAddress("alice@example.com")
    .SetTemplate("d-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")
    .AddSubstitution("FirstName", "Alice")
    .Send();
```

`IDynamicTemplateService.GetTemplateVersion(templateId, versionId, cancellationToken)` fetches a
template version from the SendGrid API.

### Attachments

Attachments can be added at the message level with `AddAttachments`, or per recipient through
`EmailRecipient.Attachments`. Three attachment types are supported:

```csharp
// Stream
message.AddAttachments(new EmailAttachmentStream(stream, "report.pdf", "application/pdf"));

// Byte array
message.AddAttachments(new EmailAttachmentBytes(bytes, "report.pdf", "application/pdf"));

// Plain string, encoded to base64 by the library
message.AddAttachments(new EmailAttachmentString("Some text.", "notes.txt"));

// String that is already base64 encoded, sent as is
message.AddAttachments(new EmailAttachmentString(base64Content, "report.pdf", "application/pdf", isBase64Encoded: true));
```

Each attachment is base64 encoded once and the encoded content is reused for every recipient.
When `IsBase64Encoded` is `true` on an `EmailAttachmentString`, the content is passed to SendGrid
without re-encoding.

### Other options

| Method | Effect |
| --- | --- |
| `SetReplyTo` | Sets the reply-to address. |
| `AddCategory` / `AddCategories` | Adds SendGrid categories. |
| `AddHeader` | Adds a custom header. |
| `SetDeliveryTime` | Schedules delivery for a future time. |
| `SetBatchId` | Uses an existing batch ID. `Send(substitute, batch: true)` requests a new batch ID from SendGrid and applies it to every recipient's request. |
| `SetTestMode` | Enables SendGrid sandbox mode for this message. Defaults to `UseTestMode` from options. |
| `SetTracking` / `SetTrackingClicks` / `SetTrackingOpens` | Controls click and open tracking. Both default to enabled. |

## Build and test

```shell
dotnet build
dotnet test
```

The tests send real messages through SendGrid. They read the API key from
`Southport.Messaging.Email.SendGrid.Test/appsettings.json` or from the `APIKEY` and
`USETESTMODE` environment variables.

## Contributing

Issues and pull requests are welcome at
[github.com/southportsolutions/Southport.Messaging.Email.SendGrid](https://github.com/southportsolutions/Southport.Messaging.Email.SendGrid).
