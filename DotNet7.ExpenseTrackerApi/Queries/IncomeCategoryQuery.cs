namespace DotNet7.ExpenseTrackerApi.Queries;

public static class IncomeCategoryQuery
{
    #region GetIncomeCategoryListQuery

    public static string GetIncomeCategoryListQuery()
    {
        return @"SELECT [IncomeCategoryId]
      ,[IncomeCategoryName]
      ,[IsActive]
  FROM [dbo].[Income_Category] WHERE IsActive = @IsActive ORDER BY IncomeCategoryId DESC";
    }

    #endregion

    #region CreateIncomeCategoryQuery

    public static string CreateIncomeCategoryQuery()
    {
        return @"INSERT INTO Income_Category (IncomeCategoryName, IsActive)
VALUES (@IncomeCategoryName, @IsActive)";
    }

    #endregion

    #region CheckCreateIncomeCategoryDuplicateQuery

    public static string CheckCreateIncomeCategoryDuplicateQuery()
    {
        return @"SELECT [IncomeCategoryId]
      ,[IncomeCategoryName]
      ,[IsActive]
  FROM [dbo].[Income_Category] WHERE IncomeCategoryName = @IncomeCategoryName AND IsActive = @IsActive";
    }

    #endregion

    #region CheckIncomeCategoryDuplicateQuery

    public static string CheckIncomeCategoryDuplicateQuery()
    {
        return @"SELECT [IncomeCategoryId]
      ,[IncomeCategoryName]
      ,[IsActive]
            FROM[dbo].[Income_Category] WHERE IncomeCategoryName = @IncomeCategoryName AND
          IsActive = @IsActive AND IncomeCategoryId != @IncomeCategoryId";
    }

    #endregion

    #region UpdateIncomeCategoryQuery

    public static string UpdateIncomeCategoryQuery()
    {
        return @"UPDATE Income_Category SET IncomeCategoryName = @IncomeCategoryName 
WHERE IncomeCategoryId = @IncomeCategoryId";
    }

    #endregion

    #region DeleteIncomeCategoryQuery

    public static string DeleteIncomeCategoryQuery()
    {
        return @"UPDATE Income_Category SET IsActive = @IsActive WHERE IncomeCategoryId = @IncomeCategoryId";
    }

    #endregion

    public static string CheckIncomeCategoryActiveQuery()
    {
        return @"SELECT IncomeCategoryId, IncomeCategoryName, IsActive
FROM Income_Category WHERE IncomeCategoryId = @IncomeCategoryId AND IsActive = @IsActive";
    }
}
