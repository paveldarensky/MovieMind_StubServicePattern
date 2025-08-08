using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServiceStub.Models
{
    public class MovieInput
    {
        [Range(1920, 2025)]
        public int Year { get; set; }

        [Range(30, 300)]
        public int Duration { get; set; }

        [Range(0.0, 10.0)]
        public double Rating { get; set; }

        [Required]
        public List<string> Genres { get; set; }
    }
}
