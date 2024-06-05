using System.ComponentModel.DataAnnotations;

namespace DotNet7.ExpenseTrackerApi.BlazorWasm.Models.User;

public class RegisterRequestModel
{
    public string UserName { get; set; } = null!;
    [EmailAddress]
    public string Email { get; set; } = null!;
    [MaxLength(12)]
    [MinLength(6)]
    public string Password { get; set; } = null!;
    public DateTime? DOB { get; set; }
    public string Gender { get; set; } = null!;
}