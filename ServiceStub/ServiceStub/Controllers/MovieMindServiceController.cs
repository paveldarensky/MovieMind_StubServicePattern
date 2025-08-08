using Microsoft.AspNetCore.Mvc;
using ServiceStub.Models;
using ServiceStub.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceStub.Controllers
{
    public class MovieMindServiceController : Controller
    {
        private readonly IMovieRecommendationService _recommendationService;
        private readonly IConfiguration _configuration;

        public MovieMindServiceController(IMovieRecommendationService recommendationService, IConfiguration configuration)
        {
            _recommendationService = recommendationService;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.UseStub = _configuration.GetValue<bool>("UseStubRecommendation");
            ViewBag.ServiceInfo = ViewBag.UseStub
                ? "<div class='alert alert-warning'>" +
                  "<strong>⚠ Используется StubMovieRecommendationService</strong><br/>" +
                  "Алгоритм рекомендаций временно заменён на демонстрационный.<br/>" +
                  "Подбор осуществляется из заготовленного списка по жанрам." +
                  "</div>"
                : "<div class='alert alert-success'>" +
                  "<strong>✔ Используется RealMovieRecommendationService</strong>" +
                  "</div>";

            return View(new MovieInput());
        }

        [HttpPost]
        public async Task<IActionResult> Recommend(MovieInput input)
        {
            ViewBag.UseStub = _configuration.GetValue<bool>("UseStubRecommendation");
            ViewBag.ServiceInfo = ViewBag.UseStub
                ? "<div class='alert alert-warning'>" +
                  "<strong>⚠ Используется StubMovieRecommendationService</strong><br/>" +
                  "Алгоритм рекомендаций временно заменён на демонстрационный.<br/>" +
                  "Подбор осуществляется из заготовленного списка по жанрам." +
                  "</div>"
                : "<div class='alert alert-success'>" +
                  "<strong>✔ Используется RealMovieRecommendationService</strong>" +
                  "</div>";

            if (_configuration.GetValue<bool>("UseStubRecommendation"))
            {
                ModelState.Remove(nameof(MovieInput.Year));
                ModelState.Remove(nameof(MovieInput.Duration));
                ModelState.Remove(nameof(MovieInput.Rating));
            }


            if (!ModelState.IsValid || input.Genres == null || input.Genres.Count == 0)
            {
                ModelState.AddModelError("", "Пожалуйста, укажите все параметры, включая жанры.");
                return View("Index", input);
            }

            var recommendations = await _recommendationService.GetRecommendationsAsync(input);

            if (recommendations.Count == 0)
            {
                ModelState.AddModelError("", "Ошибка при получении рекомендаций.");
            }


            // Сохраняем 50 фильмов в Session
            HttpContext.Session.SetString("AllRecommendations", System.Text.Json.JsonSerializer.Serialize(recommendations));
            HttpContext.Session.SetInt32("Offset", 5); // первые 5 уже покажем

            ViewBag.Recommendations = recommendations.Take(5).ToList();
            ViewBag.ShowMore = recommendations.Count > 5;
            // ViewBag.Recommendations = recommendations;
            return View("Index", input);
        }

        [HttpPost]
        public IActionResult RecommendMore()
        {
            var allJson = HttpContext.Session.GetString("AllRecommendations");
            if (string.IsNullOrEmpty(allJson))
                return RedirectToAction("Index");

            var all = System.Text.Json.JsonSerializer.Deserialize<List<MovieRecommendation>>(allJson);
            var offset = HttpContext.Session.GetInt32("Offset") ?? 0;
            var nextBatch = all.Skip(offset).Take(5).ToList();
            HttpContext.Session.SetInt32("Offset", offset + 5);

            ViewBag.Recommendations = nextBatch;
            ViewBag.ShowMore = all.Count > offset + 5;
            ViewBag.AppendMode = true;

            return View("Index");
        }
    }

    public class MovieRecommendation
    {
        public string Title { get; set; }
        public double Distance { get; set; }
    }
}


