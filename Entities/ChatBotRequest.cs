using System.Text.Json.Serialization;

namespace Entities
{
    public class ChatBotRequest
    {
        [JsonPropertyName("question")]
        public string? Question { get; set; }
    }
}
