namespace DotNet7.ExpenseTrackerApi.Models.Entities;

public class ExpenseCategoryModel
{
    public long ExpenseCategoryId { get; set; }
    public string ExpenseCategoryName { get; set; } = null!;
    public bool IsActive { get; set; }
}