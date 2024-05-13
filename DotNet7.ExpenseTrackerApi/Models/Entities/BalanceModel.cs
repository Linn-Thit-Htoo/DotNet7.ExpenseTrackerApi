using System.ComponentModel.DataAnnotations;

namespace DotNet7.ExpenseTrackerApi.Models.Entities;

public class BalanceModel
{
    [Key]
    public long BalanceId { get; set; }
    public long UserId { get; set; }
    public long Amount { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
}