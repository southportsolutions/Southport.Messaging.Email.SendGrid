using System.Text.Json.Serialization;

namespace Southport.Messaging.Email.SendGrid.Templates.Models;

public class DynamicTemplateVersion
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; }
    [JsonPropertyName("active")]
    public int Active { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("html_content")]
    public string HtmlContent { get; set; }
    [JsonPropertyName("plain_content")]
    public string PlanContent { get; set; }
    [JsonPropertyName("generate_plain_content")]
    public bool GeneratedPlanContent { get; set; }
    [JsonPropertyName("subject")]
    public string Subject { get; set; }
    [JsonPropertyName("editor")]
    public string Editor { get; set; }
    [JsonPropertyName("test_data")]
    public string TestData { get; set; }
    [JsonPropertyName("updated_at")]
    public string UpdatedAt { get; set; }
    [JsonPropertyName("thumbnail_url")]
    public string ThumbnailUrl { get; set; }
}