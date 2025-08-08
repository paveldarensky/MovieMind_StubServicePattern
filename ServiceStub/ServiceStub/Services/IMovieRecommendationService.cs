using ServiceStub.Controllers;
using ServiceStub.Models;

namespace ServiceStub.Services
{
    public interface IMovieRecommendationService
    {
        Task<List<MovieRecommendation>> GetRecommendationsAsync(MovieInput input);
    }
}
