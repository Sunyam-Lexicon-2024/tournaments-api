using System.ComponentModel.DataAnnotations;

namespace Tournaments.Core.Entities;

public class TournamentEditAPIModel(string title) : IBaseAPIModel
{
    public int Id { get; set; }
    [Required]
    [MinLength(10, ErrorMessage = "Title must be at least 10 characters")]
    [MaxLength(25, ErrorMessage = "Title cannot exceed 25 characters")]
    public string Title { get; set; } = title;
    public DateOnly StartDate { get; set; }
}