namespace DotNet7.ExpenseTrackerApi.Models.Entities;

public class BalanceModel
{
    public long BalanceId { get; set; }
    public long UserId { get; set; }
    public string Amount { get; set; } = null!;
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
}