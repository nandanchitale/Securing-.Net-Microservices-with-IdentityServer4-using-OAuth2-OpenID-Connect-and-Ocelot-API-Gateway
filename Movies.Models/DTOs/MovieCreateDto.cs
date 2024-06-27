using System.ComponentModel.DataAnnotations;

namespace Movies.API.DTO;

public class MovieDetailDto
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string Genre { get; set; }
    [Required]
    public DateTime ReleaseDate { get; set; }
    public string ImageUrl { get; set; }
    public string Rating { get; set; }
    [Required]
    public string Owner { get; set; }
}