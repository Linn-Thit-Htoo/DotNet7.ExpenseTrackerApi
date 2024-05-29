namespace DotNet7.ExpenseTrackerApi.Models.RequestModels.Balance;

public class BalanceRequestModel
{
    public long UserId { get; set; }
    public decimal Amount { get; set; }
}
