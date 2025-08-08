using ServiceStub.Controllers;
using ServiceStub.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ServiceStub.Services
{
    public class RealMovieRecommendationService : IMovieRecommendationService
    {
        private readonly HttpClient _httpClient;

        public RealMovieRecommendationService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<MovieRecommendation>> GetRecommendationsAsync(MovieInput input)
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5000/recommend", input);

            if (!response.IsSuccessStatusCode)
            {
                return new List<MovieRecommendation>(); // Или можно бросить исключение
            }

            var recommendations = await response.Content.ReadFromJsonAsync<List<MovieRecommendation>>();
            return recommendations ?? new List<MovieRecommendation>();
        }
    }
}
