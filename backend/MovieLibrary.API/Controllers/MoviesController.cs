using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MovieLibrary.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IHttpClientFactory _http;

    public MoviesController(AppDbContext db, IHttpClientFactory http)
    {
        _db = db;
        _http = http;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _db.Movies.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var movie = await _db.Movies.FindAsync(id);
        return movie == null ? NotFound(new { message = "Film nie znaleziony" }) : Ok(movie);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Movie movie)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        _db.Movies.Add(movie);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = movie.Id }, movie);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Movie movie)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        var existing = await _db.Movies.FindAsync(id);
        if (existing == null) 
            return NotFound(new { message = "Film nie znaleziony" });
        
        existing.Title = movie.Title;
        existing.Year = movie.Year;
        existing.Director = movie.Director;
        existing.Rate = movie.Rate;
        
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var movie = await _db.Movies.FindAsync(id);
        if (movie == null) 
            return NotFound(new { message = "Film nie znaleziony" });
        
        _db.Movies.Remove(movie);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("import")]
    public async Task<IActionResult> Import()
    {
        try
        {
            var client = _http.CreateClient();
            var response = await client.GetFromJsonAsync<List<ExternalMovie>>(
                "https://filmy.programdemo.pl/MyMovies");
            
            if (response == null || response.Count == 0)
                return Ok(new { added = 0, message = "Brak filmów do zaimportowania" });
            
            int added = 0, skipped = 0;
            
            foreach (var ext in response)
            {
                if (!await _db.Movies.AnyAsync(m => m.ExternalApiId == ext.Id))
                {
                    _db.Movies.Add(new Movie
                    {
                        Title = ext.Title,
                        Director = ext.Director,
                        Year = ext.Year,
                        Rate = ext.Rate,
                        ExternalApiId = ext.Id
                    });
                    added++;
                }
                else
                {
                    skipped++;
                }
            }
            
            await _db.SaveChangesAsync();
            return Ok(new { added, skipped, message = $"Zaimportowano {added} filmów, pominięto {skipped}" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Błąd: {ex.Message}" });
        }
    }
}
public class ExternalMovie
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Director { get; set; }
    public int Year { get; set; }
    public int Rate { get; set; }
}