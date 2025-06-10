using System;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfixBot
{
    public class Fetch
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public const string API_URL = "https://infixchain-api.vercel.app";

        public static async Task<JsonElement> GetAsync(string endpoint, Dictionary<string, string> parameters = null)
        {
            var query = parameters != null ? $"?{string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"))}" : "";
            var response = await _httpClient.GetAsync($"{API_URL}/{endpoint}{query}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(content);
        }
    }
}
