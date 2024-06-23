using System.ComponentModel.DataAnnotations;

namespace Movies.Models;

public class Movies
{
    [Key]
    [Required]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Genre { get; set; }
    public string? Rating { get; set; } = null;
    public string? ImageUrl { get; set; } = null;
    [Required]
    public DateTime ReleaseDate { get; set; }
    [Required]
    public string Owner { get; set; }
    [Required]
    public string Status { get; set; } = Utils.Constants.Status.ACTIVE;
}