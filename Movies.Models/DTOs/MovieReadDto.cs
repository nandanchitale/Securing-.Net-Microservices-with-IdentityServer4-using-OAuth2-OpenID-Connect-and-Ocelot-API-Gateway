using System.ComponentModel.DataAnnotations;

namespace Movies.API.DTO;

public class MovieReadDto
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Genre { get; set; }
    [Required]
    public DateTime ReleaseDate { get; set; }
    [Required]
    public string Owner { get; set; }
}