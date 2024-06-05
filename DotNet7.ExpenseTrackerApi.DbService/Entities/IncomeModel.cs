using System.ComponentModel.DataAnnotations;

namespace DotNet7.ExpenseTrackerApi.Models.Entities;

public class IncomeModel
{
    [Key]
    public long IncomeId { get; set; }
    public long UserId { get; set; }
    public long IncomeCategoryId { get; set; }
    public long Amount { get; set; }
    public string CreateDate { get; set; } = null!;
    public bool IsActive { get; set; }
}