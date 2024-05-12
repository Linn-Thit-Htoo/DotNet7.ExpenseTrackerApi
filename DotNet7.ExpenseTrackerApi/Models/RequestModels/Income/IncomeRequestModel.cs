﻿namespace DotNet7.ExpenseTrackerApi.Models.RequestModels.Income;

public class IncomeRequestModel
{
    public long IncomeCategoryId { get; set; }
    public long UserId { get; set; }
    public long Amount { get; set; }
    public string CreateDate { get; set; } = null!;
}
