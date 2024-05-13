namespace DotNet7.ExpenseTrackerApi.Queries;

public static class ExpenseCategoryQuery
{
    #region GetExpenseCategoryListQuery

    public static string GetExpenseCategoryListQuery()
    {
        return @"SELECT [ExpenseCategoryId]
      ,[ExpenseCategoryName]
      ,[IsActive]
  FROM [dbo].[Expense_Category] WHERE IsActive = @IsActive ORDER BY ExpenseCategoryId DESC";
    }

    #endregion

    #region CheckCreateExpenseCategoryDuplicateQuery

    public static string CheckCreateExpenseCategoryDuplicateQuery()
    {
        return @"SELECT [ExpenseCategoryId]
      ,[ExpenseCategoryName]
      ,[IsActive]
  FROM [dbo].[Expense_Category] WHERE ExpenseCategoryName = @ExpenseCategoryName AND IsActive = @IsActive";
    }

    #endregion

    #region CreateExpenseCategoryQuery

    public static string CreateExpenseCategoryQuery()
    {
        return @"INSERT INTO Expense_Category (ExpenseCategoryName, IsActive) VALUES (@ExpenseCategoryName, @IsActive)";
    }

    #endregion

    #region CheckUpdateExpenseCategoryDuplicateQuery

    public static string CheckUpdateExpenseCategoryDuplicateQuery()
    {
        return @"SELECT [ExpenseCategoryId]
      ,[ExpenseCategoryName]
      ,[IsActive]
  FROM [dbo].[Expense_Category] WHERE ExpenseCategoryName = @ExpenseCategoryName AND
IsActive = @IsActive AND
ExpenseCategoryId != @ExpenseCategoryId";
    }

    #endregion

    #region UpdateExpenseCategoryQuery

    public static string UpdateExpenseCategoryQuery()
    {
        return @"UPDATE Expense_Category SET ExpenseCategoryName = @ExpenseCategoryName WHERE
ExpenseCategoryId = @ExpenseCategoryId";
    }

    #endregion

    #region CheckExpenseCategoryQuery

    public static string CheckExpenseCategoryQuery()
    {
        return @"SELECT [ExpenseId]
      ,[ExpenseCategoryId]
      ,[Amount]
      ,[IsActive]
  FROM [dbo].[Expense] WHERE ExpenseCategoryId = @ExpenseCategoryId";
    }

    #endregion

    #region DeleteExpenseCategoryQuery

    public static string DeleteExpenseCategoryQuery()
    {
        return @"UPDATE Expense_Category SET IsActive = @IsActive WHERE ExpenseCategoryId = @ExpenseCategoryId";
    }

    #endregion

    #region GetCheckExpenseCategoryActiveQuery

    public static string GetCheckExpenseCategoryActiveQuery()
    {
        return @"SELECT [ExpenseCategoryId]
      ,[ExpenseCategoryName]
      ,[IsActive]
  FROM [dbo].[Expense_Category] WHERE ExpenseCategoryId = @ExpenseCategoryId AND IsActive = @IsActive";
    }

    #endregion
}
