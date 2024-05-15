using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet7.ExpenseTrackerApi.Models.Entities;

[Table("Expense_Category")]
public class ExpenseCategoryModel
{
    [Key]
    public long ExpenseCategoryId { get; set; }
    public string ExpenseCategoryName { get; set; } = null!;
    public bool IsActive { get; set; }
}