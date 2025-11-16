using System.ComponentModel.DataAnnotations;

namespace MovieLibrary.API;

public class Movie
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Tytuł jest wymagany")]
    [MaxLength(200, ErrorMessage = "Tytuł może mieć maksymalnie 200 znaków")]
    public string Title { get; set; } = "";
    
    [Range(1900, 2200, ErrorMessage = "Rok musi być między 1900 a 2200")]
    public int Year { get; set; }
    
    public string? Director { get; set; }
    public int? Rate { get; set; }
    public int? ExternalApiId { get; set; }
}