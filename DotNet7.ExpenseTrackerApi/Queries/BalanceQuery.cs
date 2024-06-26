﻿namespace DotNet7.ExpenseTrackerApi.Queries;

public static class BalanceQuery
{
    #region Get Balance By User Id

    public static string GetBalanceByUserId()
    {
        return @"SELECT [BalanceId]
      ,[UserId]
      ,[Amount]
      ,[CreateDate]
      ,[UpdateDate]
  FROM [dbo].[Balance] WHERE UserId = @UserId";
    }

    #endregion

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