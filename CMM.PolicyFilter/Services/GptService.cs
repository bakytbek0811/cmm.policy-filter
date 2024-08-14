using System.Text;
using System.Text.Json;
using CMM.PolicyFilter.Entities;

namespace CMM.PolicyFilter.Services
{
    public interface IGptService
    {
        Task<GptResponse> GenerateText(GptMessage[] messages);
    }

    public class GptService : IGptService
    {
        private readonly IConfiguration _configuration;

        public GptService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<GptResponse> GenerateText(GptMessage[] messages)
        {
            var url = "https://api.openai.com/v1/chat/completions";
            var gptModel = "gpt-4o";
            var token = Environment.GetEnvironmentVariable("GPT_API_KEY");

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", $"Bearer {token}");

            var json = JsonSerializer.Serialize(new
            {
                messages,
                model = gptModel
            });

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            var response = httpClient.Send(request);

            response.EnsureSuccessStatusCode();

            var data = JsonSerializer.Deserialize<GptResponse>(await response.Content.ReadAsStringAsync());

            if (data == null)
            {
                throw new Exception("Something went wrong.");
            }
            
            return data;
        }
    }
}