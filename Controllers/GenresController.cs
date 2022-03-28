using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Services;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            IEnumerable<Genre> genres = await _genreService.GetAll();
            return Ok(genres);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(GenreDto dto)
        {
            Genre genre = new() { Name = dto.Name };
            await _genreService.Add(genre);

            return Ok(genre);
        }

        [HttpPut(template: "{id}")]
        public async Task<IActionResult> UpdateAsync(byte id, [FromBody] GenreDto dto)
        {
            Genre genre = await _genreService.GetById(id);
            if (genre is null)
                return NotFound($"No genre was found with Id: {id}");

            genre.Name = dto.Name;
            _genreService.Update(genre);

            return Ok(genre);
        }

        [HttpDelete(template: "{id}")]
        public async Task<IActionResult> DeleteAsync(byte id)
        {
            Genre genre = await _genreService.GetById(id);
            if (genre is null)
                return NotFound($"No genre was found with Id: {id}");

            _genreService.Delete(genre);

            return Ok($"Genre with Id: {id} was deleted");
        }
    }
}
