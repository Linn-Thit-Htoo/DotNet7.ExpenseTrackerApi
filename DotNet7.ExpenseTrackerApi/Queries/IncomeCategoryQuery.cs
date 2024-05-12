namespace DotNet7.ExpenseTrackerApi.Queries;

public static class IncomeCategoryQuery
{
    public static string GetIncomeCategoryListQuery()
    {
        return @"SELECT [IncomeCategoryId]
      ,[IncomeCategoryName]
      ,[IsActive]
  FROM [dbo].[Income_Category] WHERE IsActive = @IsActive ORDER BY IncomeCategoryId DESC";
    }

    public static string CreateIncomeCategoryQuery()
    {
        return @"INSERT INTO Income_Category (IncomeCategoryName, IsActive)
VALUES (@IncomeCategoryName, @IsActive)";
    }

    public static string CheckCreateIncomeCategoryDuplicateQuery()
    {
        return @"SELECT [IncomeCategoryId]
      ,[IncomeCategoryName]
      ,[IsActive]
  FROM [dbo].[Income_Category] WHERE IncomeCategoryName = @IncomeCategoryName AND IsActive = @IsActive";
    }

    public static string CheckIncomeCategoryDuplicateQuery()
    {
        return @"SELECT [IncomeCategoryId]
      ,[IncomeCategoryName]
      ,[IsActive]
            FROM[dbo].[Income_Category] WHERE IncomeCategoryName = @IncomeCategoryName AND
          IsActive = @IsActive AND IncomeCategoryId != @IncomeCategoryId";
    }

    public static string UpdateIncomeCategoryQuery()
    {
        return @"UPDATE Income_Category SET IncomeCategoryName = @IncomeCategoryName 
WHERE IncomeCategoryId = @IncomeCategoryId";
    }

    public static string CheckIncomeCategoryExistsQuery()
    {
        return @"SELECT [IncomeId]
      ,[IncomeCategoryId]
      ,[Amount]
      ,[IsActive]
  FROM [dbo].[Income] WHERE IncomeCategoryId = @IncomeCategoryId";
    }

    public static string DeleteIncomeCategoryQuery()
    {
        return @"UPDATE Income_Category SET IsActive = @IsActive WHERE IncomeCategoryId = @IncomeCategoryId";
    }
}
