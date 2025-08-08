using ServiceStub.Controllers;
using ServiceStub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceStub.Services
{
    public class StubMovieRecommendationService : IMovieRecommendationService
    {
        private readonly Dictionary<string, List<string>> _genreMovies = new()
        {
            { "Drama", new List<string> { "The Shawshank Redemption", "Forrest Gump", "The Green Mile", "Fight Club", "The Godfather", "12 Angry Men", "The Pianist", "A Beautiful Mind", "The Pursuit of Happyness", "Dead Poets Society" } },
            { "Comedy", new List<string> { "The Intouchables", "Back to the Future", "Groundhog Day", "The Grand Budapest Hotel", "Superbad", "The Hangover", "Monty Python and the Holy Grail", "Mean Girls", "Zombieland", "Bridesmaids" } },
            { "Action", new List<string> { "Mad Max: Fury Road", "John Wick", "The Dark Knight", "Inception", "Gladiator", "Die Hard", "The Matrix", "Avengers: Endgame", "Léon", "Speed" } },
            { "Adventure", new List<string> { "The Lord of the Rings", "Pirates of the Caribbean", "Indiana Jones", "Interstellar", "Jurassic Park", "The Revenant", "The Hobbit", "Cast Away", "Life of Pi", "Up" } },
            { "Crime", new List<string> { "Pulp Fiction", "The Godfather", "Goodfellas", "Se7en", "The Departed", "Heat", "City of God", "The Usual Suspects", "Prisoners", "The Wolf of Wall Street" } }
        };

        public Task<List<MovieRecommendation>> GetRecommendationsAsync(MovieInput input)
        {
            var selectedGenres = input.Genres?.Intersect(_genreMovies.Keys).ToList() ?? new();
            var random = new Random();
            var recommendations = new List<MovieRecommendation>();

            foreach (var genre in selectedGenres)
            {
                var films = _genreMovies[genre];
                var film = films[random.Next(films.Count)];
                recommendations.Add(new MovieRecommendation
                {
                    Title = film + " (" + genre + ")",
                    Distance = Math.Round(random.NextDouble(), 2) // Заглушка значения сходства
                });
            }

            return Task.FromResult(recommendations);
        }
    }
}
