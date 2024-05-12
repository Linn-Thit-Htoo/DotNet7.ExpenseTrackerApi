namespace DotNet7.ExpenseTrackerApi.Queries;

public static class ExpenseQuery
{
    #region GetExpenseListQuery

    public static string GetExpenseListQuery()
    {
        return @"SELECT ExpenseId, Expense_Category.ExpenseCategoryName, Users.UserName, Amount, Expense.IsActive,
Expense.CreateDate
FROM Expense
INNER JOIN Expense_Category ON Expense.ExpenseCategoryId = Expense_Category.ExpenseCategoryId
INNER JOIN Users ON Expense.UserId = Users.UserId
WHERE Expense.IsActive = @IsActive
ORDER BY ExpenseId DESC";
    }

    #endregion

    #region GetExpenseListByUserIdQuery

    public static string GetExpenseListByUserIdQuery()
    {
        return @"SELECT ExpenseId, Expense_Category.ExpenseCategoryName, Users.UserName, Amount, Expense.IsActive,
Expense.CreateDate
FROM Expense
INNER JOIN Expense_Category ON Expense.ExpenseCategoryId = Expense_Category.ExpenseCategoryId
INNER JOIN Users ON Expense.UserId = Users.UserId
WHERE Expense.IsActive = @IsActive AND Expense.UserId = @UserId
ORDER BY ExpenseId DESC";
    }

    #endregion

    #region CreateExpenseQuery

    public static string CreateExpenseQuery()
    {
        return @"INSERT INTO Expense (ExpenseCategoryId, UserId, Amount, CreateDate, IsActive)
VALUES (@ExpenseCategoryId, @UserId, @Amount, @CreateDate, @IsActive)";
    }

    #endregion

    #region UpdateExpenseQuery

    public static string UpdateExpenseQuery()
    {
        return @"UPDATE Expense SET ExpenseCategoryId = @ExpenseCategoryId, 
Amount = @Amount WHERE ExpenseId = @ExpenseId AND UserId = @UserId";
    }

    #endregion

    #region DeleteExpenseQuery

    public static string DeleteExpenseQuery()
    {
        return @"UPDATE Expense SET IsActive = @IsActive WHERE ExpenseId = @ExpenseId";
    }

    #endregion
}
