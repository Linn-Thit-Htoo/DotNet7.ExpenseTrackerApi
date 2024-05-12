namespace DotNet7.ExpenseTrackerApi.Models.Entities;

public class IncomeCategoryModel
{
    public long IncomeCategoryId { get; set; }
    public string IncomeCategoryName { get; set; } = null!;
    public bool IsActive { get; set; }
}