namespace DotNet7.ExpenseTrackerApi.Models.RequestModels.Income;

public class UpdateIncomeRequestModel
{
    public long IncomeCategoryId { get; set; }
    public long UserId { get; set; }
    public long Amount { get; set; }
}