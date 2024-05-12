namespace DotNet7.ExpenseTrackerApi.Queries;

public static class IncomeQuery
{
    #region GetIncomeListQuery

    public static string GetIncomeListQuery()
    {
        return @"SELECT Income.IncomeId, Income.CreateDate, Users.UserName, Income_Category.IncomeCategoryName,
Income.Amount, Income.IsActive
FROM Income
INNER JOIN Users ON Income.UserId = Users.UserId
INNER JOIN Income_Category ON Income.IncomeCategoryId = Income_Category.IncomeCategoryId
WHERE Income.IsActive = @IsActive
ORDER BY IncomeId DESC"; ;
    }

    #endregion

    #region GetIncomeListByUserIdQuery

    public static string GetIncomeListByUserIdQuery()
    {
        return @"SELECT Income.IncomeId, Income.CreateDate, Users.UserName, Income_Category.IncomeCategoryName,
Income.Amount, Income.IsActive
FROM Income
INNER JOIN Users ON Income.UserId = Users.UserId
INNER JOIN Income_Category ON Income.IncomeCategoryId = Income_Category.IncomeCategoryId
WHERE Income.IsActive = @IsActive AND Income.UserId = @UserId
ORDER BY IncomeId DESC";
    }

    #endregion

    #region CreateIncomeQuery

    public static string CreateIncomeQuery()
    {
        return @"INSERT INTO Income (IncomeCategoryId, UserId, Amount, CreateDate, IsActive)
VALUES (@IncomeCategoryId, @UserId, @Amount, @CreateDate, @IsActive)";
    }

    #endregion

    #region UpdateIncomeQuery

    public static string UpdateIncomeQuery()
    {
        return @"UPDATE Income SET IncomeCategoryId = @IncomeCategoryId,
Amount = @Amount WHERE IncomeId = @IncomeId AND UserId = @UserId";
    }

    #endregion

    #region DeleteIncomeQuery

    public static string DeleteIncomeQuery()
    {
        return @"UPDATE Income SET IsActive = @IsActive WHERE IncomeId = @IncomeId";
    }

    #endregion
}
