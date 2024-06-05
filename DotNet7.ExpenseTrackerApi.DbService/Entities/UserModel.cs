using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet7.ExpenseTrackerApi.DbService.Entities;

[Table("Users")]
public class UserModel
{
    [Key]
    public long UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string UserRole { get; set; } = null!;
    public string DOB { get; set; } = null!;
    public string Gender { get; set; } = null!;
    public bool IsActive { get; set; }
}