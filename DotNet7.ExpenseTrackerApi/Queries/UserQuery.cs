namespace DotNet7.ExpenseTrackerApi.Queries;

public static class UserQuery
{
    #region GetRegisterQuery

    public static string GetRegisterQuery()
    {
        return @"INSERT INTO Users (UserName, Email, Password, UserRole, DOB, Gender, IsActive)
VALUES (@UserName, @Email, @Password, @UserRole, @DOB, @Gender, @IsActive);
SELECT SCOPE_IDENTITY();";
    }

    #endregion

    #region GetLoginQuery

    public static string GetLoginQuery()
    {
        return @"SELECT [UserId]
      ,[UserName]
      ,[Email]
      ,[UserRole]
      ,[DOB]
      ,[Gender]
      ,[IsActive]
  FROM [dbo].[Users] WHERE Email = @Email AND Password = @Password AND IsActive = @IsActive";
    }

    #endregion

    #region GetDuplicateEmailQuery

    public static string GetDuplicateEmailQuery()
    {
        return @"SELECT [UserId]
      ,[UserName]
      ,[Email]
      ,[UserRole]
      ,[DOB]
      ,[Gender]
      ,[IsActive]
  FROM [dbo].[Users] WHERE Email = @Email AND IsActive = @IsActive";
    }

    #endregion

    #region CheckUserEixstsQuery

    public static string CheckUserEixstsQuery()
    {
        return @"SELECT [UserId]
      ,[UserName]
      ,[Email]
      ,[UserRole]
      ,[DOB]
      ,[Gender]
      ,[IsActive]
  FROM [dbo].[Users] WHERE UserId = @UserId AND IsActive = @IsActive";
    }

    #endregion
}
