using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Repositories
{
    public class ChatBotRepository : IChatBotRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;

        public ChatBotRepository(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = configuration["ChatBotService:BaseUrl"] ?? "http://localhost:8001";
        }

        public async Task<ChatBotResponse> AskQuestionAsync(ChatBotRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync($"{_baseUrl}/chat", request);
            
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

        public async Task<bool> UpdateDatabaseAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync($"{_baseUrl}/ingest", null);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"External Python Chat API returned status code {response.StatusCode} on ingestion. Details: {errorDetails}");
            }
            
            return true;
        }

        public async Task<bool> SyncProductsAsync(List<Product> products)
        {
            var client = _httpClientFactory.CreateClient();
            var payload = products.Select(p => new
            {
                id = $"product_{p.Id}",
                text = $"{p.Name}. Price: ${p.Price:F2}. {p.Description}"
            }).ToList();

            var response = await client.PostAsJsonAsync($"{_baseUrl}/products", payload);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"External Python Chat API returned status code {response.StatusCode} on products sync. Details: {errorDetails}");
            }
            
            return true;
        }
    }
}

