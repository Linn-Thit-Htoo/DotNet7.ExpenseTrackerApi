using System.ComponentModel.DataAnnotations;

namespace DotNet7.ExpenseTrackerApi.Models.Entities;

public class ExpenseModel
{
    [Key]
    public long ExpenseId { get; set; }
    public long ExpenseCategoryId { get; set; }
    public long UserId { get; set; }
    public long Amount { get; set; }
    public string CreateDate { get; set; } = null!;
    public bool IsActive { get; set; }
}