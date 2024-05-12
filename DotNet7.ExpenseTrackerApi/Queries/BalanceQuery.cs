namespace DotNet7.ExpenseTrackerApi.Queries;

public static class BalanceQuery
{
    #region CreateBalanceQuery

    public static string CreateBalanceQuery()
    {
        return @"INSERT INTO Balance (UserId, Amount, CreateDate)
            VALUES (@UserId, @Amount, @CreateDate)";
    }

    #endregion

    #region UpdateBalanceQuery

    public static string UpdateBalanceQuery()
    {
        return @"UPDATE Balance SET Amount = @Amount, UpdateDate = @UpdateDate
WHERE UserId = @UserId";
    }

    #endregion
}