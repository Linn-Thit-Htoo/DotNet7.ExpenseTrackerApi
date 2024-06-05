using System.ComponentModel.DataAnnotations;

namespace DotNet7.ExpenseTrackerApi.Models.Setup.User;

public class LoginRequestModel
{
    [EmailAddress]
    public required string Email { get; set; }
    public required string Password { get; set; }
}