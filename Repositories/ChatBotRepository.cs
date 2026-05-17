using Entities;
using System;
using System.Net.Http;
using System.Net.Http.Json;
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
            var response = await client.PostAsJsonAsync("http://localhost:8001/chat", request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"External Python Chat API returned status code {response.StatusCode}. Details: {errorDetails}");
            }

            var result = await response.Content.ReadFromJsonAsync<ChatBotResponse>();
            if (result == null)
            {
                throw new Exception("Received empty response from the Python chat service.");
            }

            return result;
        }
    }
}

