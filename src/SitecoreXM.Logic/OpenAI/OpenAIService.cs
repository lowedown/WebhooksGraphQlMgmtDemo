using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SitecoreXM.Logic.Models;
using System.Net.Http.Headers;
using System.Text;

namespace SitecoreXM.Logic.OpenAI
{
    public class OpenAIService
    {
        private readonly ILogger<OpenAIService> logger;
        private readonly IConfiguration configuration;

        public OpenAIService(ILogger<OpenAIService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {configuration["OpenAI:ApiKey"]}");
            return client;
        }

        public async Task<string?> ChatCompletion(string input)
        {
            var prompt = @"{
                    ""model"": ""gpt-3.5-turbo"",
                      ""messages"": [
                        {
                          ""role"": ""user"",
                          ""content"": {{INPUT}}
                        }
                      ],
                      ""temperature"": 1,
                      ""max_tokens"": 256,
                      ""top_p"": 1,
                      ""frequency_penalty"": 0,
                      ""presence_penalty"": 0
                    }
            ".Replace("{{INPUT}}", JsonConvert.ToString(input));


            var request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://api.openai.com/v1/chat/completions"));
            request.Content = new StringContent(prompt, Encoding.UTF8, "application/json");
            var client = GetHttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.SendAsync(request);

            var json = response.Content.ReadAsStringAsync().Result;
            var openAiMessage = JsonConvert.DeserializeObject<OpenAIResponse>(json);

            if (openAiMessage == null)
            {
                this.logger.LogError($"OpenAI response was invalid: {json}");
                return null;
            }

            return openAiMessage.choices[0].message.content;
        }

        public async Task<Uri?> GenerateImage(string input)
        {
            var prompt = @"{
                        ""model"": ""dall-e-3"",
                        ""prompt"": {{INPUT}},
                        ""n"": 1,
                        ""size"": ""1024x1024""
                        }
            ".Replace("{{INPUT}}", JsonConvert.ToString(input));

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://api.openai.com/v1/images/generations"));
            request.Content = new StringContent(prompt, Encoding.UTF8, "application/json");

            var client = GetHttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            var response = await client.SendAsync(request);

            var json = response.Content.ReadAsStringAsync().Result;
            var imageResponse = JsonConvert.DeserializeObject<OpenAIImageResponse>(json);

            if (imageResponse == null || !imageResponse.data.Any())
            {
                logger.LogError($"OpenAI response was invalid: {json}");
                return null;
            }

            return new Uri(imageResponse.data[0].url);
        }
    }
}
