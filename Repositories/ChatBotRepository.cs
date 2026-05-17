using Entities;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Repositories
{
    public class ChatBotRepository : IChatBotRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatBotRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ChatBotResponse> AskQuestionAsync(ChatBotRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            
            // Step 1: Request an event_id from Gradio API
            var requestPayload = new { data = new[] { request.Question } };
            var jsonRequest = JsonSerializer.Serialize(requestPayload);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://atara57769-playmobil-rag.hf.space/gradio_api/call/chat", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"External API returned status code {response.StatusCode}. Details: {errorDetails}");
            }

            var eventResponseText = await response.Content.ReadAsStringAsync();
            using var eventJson = JsonDocument.Parse(eventResponseText);
            var eventId = eventJson.RootElement.GetProperty("event_id").GetString();

            // Step 2: Listen to SSE stream for the complete event
            var streamResponse = await client.GetStreamAsync($"https://atara57769-playmobil-rag.hf.space/gradio_api/call/chat/{eventId}");
            using var reader = new StreamReader(streamResponse);
            
            string line;
            bool isCompleteEvent = false;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (line.StartsWith("event: complete"))
                {
                    isCompleteEvent = true;
                }
                else if (isCompleteEvent && line.StartsWith("data: "))
                {
                    var dataJson = line.Substring("data: ".Length);
                    using var dataDoc = JsonDocument.Parse(dataJson);
                    var answer = dataDoc.RootElement[0].GetString();
                    return new ChatBotResponse { Answer = answer ?? "" };
                }
            }

            throw new Exception("Did not receive a complete event from the external API.");
        }
    }
}
