using System.Text.Json.Serialization;

namespace Entities
{
    public class ChatBotResponse
    {
        [JsonPropertyName("answer")]
        public string? Answer { get; set; }
    }
}
