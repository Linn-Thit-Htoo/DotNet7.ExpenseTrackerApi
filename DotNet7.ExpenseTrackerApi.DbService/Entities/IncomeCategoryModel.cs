using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet7.ExpenseTrackerApi.Models.Entities;

[Table("Income_Category")]
public class IncomeCategoryModel
{
    [Key]
    public long IncomeCategoryId { get; set; }
    public string IncomeCategoryName { get; set; } = null!;
    public bool IsActive { get; set; }
}