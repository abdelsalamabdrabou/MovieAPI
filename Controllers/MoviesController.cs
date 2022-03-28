using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Services;
using System.Linq;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IGenreService _genreService;

        public MoviesController(IMovieService movieService, IGenreService genreService)
        {
            _movieService = movieService;
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _movieService.GetAll();

            return Ok(movies);
        }

        [HttpGet(template: "{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _movieService.GetById(id);

            if (movie is null)
                return NotFound("No movie was found");

            return Ok(movie);
        }

        [HttpGet("GetByGenre")]
        public async Task<IActionResult> GetByGenreAsync(byte genreId)
        {
            var movies = await _movieService.GetAll(genreId);
            return Ok(movies);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
        {
            if (dto.Poster is null)
                return BadRequest("The Poster field is required.");

            if (!Utility.IsFileExtensionAllowed(dto.Poster))
                return BadRequest("Only .png and .jpg images are allowed!");

            if (!Utility.IsFileSizeAllowed(dto.Poster))
                return BadRequest("Max allowed size for poster is 1MB!");

            if (!_genreService.IsValidGenre(dto.GenreId))
                return BadRequest("Invalid genre Id!");

            var poster = await Utility.ConvertFileToByteArray(dto.Poster);

            Movie movie = new()
            {
                GenreId = dto.GenreId,
                Title = dto.Title,
                Year = dto.Year,
                Rate = dto.Rate,
                Storyline = dto.Storyline,
                Poster = poster
            };

            await _movieService.Add(movie);

            return Ok(movie);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] MovieDto dto)
        {
            var movie = await _movieService.GetById(id);
            if (movie is null)
                return NotFound($"No movie was found with this Id: {id}");

            if (!_genreService.IsValidGenre(dto.GenreId))
                return BadRequest("Invalid genre Id!");            

            if (dto.Poster is not null)
            {
                if (!Utility.IsFileExtensionAllowed(dto.Poster))
                    return BadRequest("Only .png and .jpg images are allowed!");

                if (!Utility.IsFileSizeAllowed(dto.Poster))
                    return BadRequest("Max allowed size for poster is 1MB!");

                var poster = await Utility.ConvertFileToByteArray(dto.Poster);
                movie.Poster = poster;
            }

            movie.Title = dto.Title;
            movie.Year = dto.Year;
            movie.Rate = dto.Rate;
            movie.Storyline = dto.Storyline;

            _movieService.Update(movie);

            return Ok(movie);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _movieService.GetById(id);
            if (movie is null)
                return NotFound($"No movie was found with this Id: {id}");

            _movieService.Delete(movie);

            return Ok($"Movie with Id: {id} was deleted");
        }
    }
}
