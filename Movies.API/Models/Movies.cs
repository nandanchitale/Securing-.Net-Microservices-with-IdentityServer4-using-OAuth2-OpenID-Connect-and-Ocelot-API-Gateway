using System.ComponentModel.DataAnnotations;

namespace Movies.API.Model;

public class Movie
{
    [Key]
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
    public string Status { get; set; } = Utils.Constants.Status.ACTIVE;
}