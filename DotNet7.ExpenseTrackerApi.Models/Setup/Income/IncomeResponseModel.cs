namespace DotNet7.ExpenseTrackerApi.Models.Setup.Income;

public class IncomeResponseModel
{
    public long IncomeId { get; set; }
    public string UserName { get; set; } = null!;
    public string IncomeCategoryName { get; set; } = null!;
    public long Amount { get; set; }
    public string CreateDate { get; set; } = null!;
    public bool IsActive { get; set; }
}