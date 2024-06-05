namespace DotNet7.ExpenseTrackerApi.Models.ResponseModels.User;

public class LoginResponseModel
{
    public long UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string DOB { get; set; } = null!;
    public string Gender { get; set; } = null!;
}