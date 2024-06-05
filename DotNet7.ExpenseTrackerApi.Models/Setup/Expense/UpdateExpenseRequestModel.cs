namespace DotNet7.ExpenseTrackerApi.Models.RequestModels.Expense;

public class UpdateExpenseRequestModel
{
    public long ExpenseCategoryId { get; set; }
    public long UserId { get; set; }
    public long Amount { get; set; }
}