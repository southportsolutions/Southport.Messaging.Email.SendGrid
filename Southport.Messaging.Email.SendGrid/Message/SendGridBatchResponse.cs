using Newtonsoft.Json;

namespace Southport.Messaging.Email.SendGrid.Message;

public class SendGridBatchResponse
{
    [JsonProperty("batch_id")]
    public string BatchId { get; set; }
}