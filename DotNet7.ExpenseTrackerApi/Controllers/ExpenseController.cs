﻿using DotNet7.ExpenseTrackerApi.Models.RequestModels.Expense;
using DotNet7.ExpenseTrackerApi.Models.ResponseModels.Expense;
using DotNet7.ExpenseTrackerApi.Services;
using DotNet7.ExpenseTrackerApi.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics.Eventing.Reader;

namespace DotNet7.ExpenseTrackerApi.Controllers;

public class ExpenseController : ControllerBase
{
    private readonly AdoDotNetService _service;
    private readonly IConfiguration _configuration;

    public ExpenseController(AdoDotNetService service, IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;
    }

    [HttpGet]
    [Route("/api/expense")]
    public IActionResult GetList()
    {
        try
        {
            string query = ExpenseQuery.GetExpenseListQuery();
            SqlParameter[] sqlParameters = { new("@IsActive", true) };
            List<ExpenseResponseModel> lst = _service.Query<ExpenseResponseModel>(query, sqlParameters);

            return Ok(lst);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet]
    [Route("/api/expense/{userID}")]
    public IActionResult GetExpenseListByUserId(long userID)
    {
        try
        {
            if (userID <= 0)
                return BadRequest("User Id cannot be empty.");


            string query = ExpenseQuery.GetExpenseListByUserIdQuery();
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@UserId", userID),
                new SqlParameter("@IsActive", true)
            };
            List<ExpenseResponseModel> lst = _service.Query<ExpenseResponseModel>(query, parameters.ToArray());

            return Ok(lst);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpPost]
    [Route("/api/expense")]
    public IActionResult CreateExpense([FromBody] ExpenseRequestModel requestModel)
    {
        SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
        conn.Open();
        SqlTransaction transaction = conn.BeginTransaction();

        try
        {
            #region Validation

            if (requestModel.ExpenseCategoryId <= 0)
                return BadRequest();

            if (requestModel.Amount <= 0)
                return BadRequest();

            if (requestModel.UserId <= 0)
                return BadRequest();

            if (string.IsNullOrEmpty(requestModel.CreateDate))
                return BadRequest();

            #endregion

            #region Check Expense Category is valid

            string checkExpenseCategoryActiveQuery = ExpenseCategoryQuery.GetCheckExpenseCategoryActiveQuery();
            List<SqlParameter> checkExpenseCategoryActiveParams = new()
            {
                new SqlParameter("@ExpenseCategoryId", requestModel.ExpenseCategoryId),
                new SqlParameter("@IsActive", true)
            };
            DataTable expenseCategory = _service.QueryFirstOrDefault(checkExpenseCategoryActiveQuery, checkExpenseCategoryActiveParams.ToArray());
            if (expenseCategory.Rows.Count <= 0)
                return NotFound("Expense Category Not Found or Inactive");

            #endregion

            #region Create Expense

            string query = ExpenseQuery.CreateExpenseQuery();
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@ExpenseCategoryId", requestModel.ExpenseCategoryId),
                new SqlParameter("@UserId", requestModel.UserId),
                new SqlParameter("@Amount", requestModel.Amount),
                new SqlParameter("@CreateDate", requestModel.CreateDate),
                new SqlParameter("@IsActive", true),
            };
            int result = _service.Execute(conn, transaction, query, parameters.ToArray());

            #endregion

            #region Get Old Balance

            string getOldBalanceQuery = @"SELECT [BalanceId]
      ,[UserId]
      ,[Amount]
      ,[NeededAmount]
      ,[CreateDate]
      ,[UpdateDate]
  FROM [dbo].[Balance] WHERE UserId = @UserId";
            SqlParameter[] getOldBalanceParams = { new("@UserId", requestModel.UserId) };
            DataTable oldBalanceDt = _service.QueryFirstOrDefault(getOldBalanceQuery, getOldBalanceParams);

            #endregion

            long oldBalance = Convert.ToInt64(oldBalanceDt.Rows[0]["Amount"]);
            long updatedBalance = 0;

            int normalUpdateBalanceResult = 0;
            int neededAmountUpdateResult = 0;

            #region Old Balance is greater or equal to the expense amount

            if (oldBalance >= requestModel.Amount)
            {
                updatedBalance = oldBalance - requestModel.Amount;

                string normalUpdateBalanceQuery = @"UPDATE Balance SET Amount = @Amount WHERE UserId = @UserId";
                List<SqlParameter> normalUpdateBalanceParams = new()
                {
                    new SqlParameter("@Amount", updatedBalance),
                    new SqlParameter("@UserId", requestModel.UserId)
                };
                normalUpdateBalanceResult = _service
                   .Execute(conn, transaction, normalUpdateBalanceQuery, normalUpdateBalanceParams.ToArray());
            }

            #endregion

            #region Expense amount is much larger than old balance

            if (requestModel.Amount > oldBalance)
            {
                long oldNeededAmount = 0;
                long newNeededAmount = 0;

                if (oldBalance == 0)
                {
                    #region Get Old Needed Amount

                    string getOldNeededAmountQuery = @"SELECT NeededAmount FROM Balance WHERE UserId = @UserId";
                    SqlParameter[] getOldNeededAmountParams = { new("@UserId", requestModel.UserId) };
                    DataTable oldNeededAmountDt = _service.QueryFirstOrDefault(getOldNeededAmountQuery, getOldNeededAmountParams);

                    #endregion

                    oldNeededAmount = Convert.ToInt64(oldNeededAmountDt.Rows[0]["NeededAmount"]);
                }

                if (oldNeededAmount != 0)
                {
                    newNeededAmount = requestModel.Amount + oldNeededAmount;
                }

                #region Update New Needed Amount

                string neededAmountUpdateQuery = @"UPDATE Balance SET Amount = @Amount, NeededAmount = @NeededAmount WHERE UserId = @UserId";
                List<SqlParameter> neededAmountUpdateParams = new()
                {
                    new SqlParameter("@Amount", updatedBalance),
                    new SqlParameter("@NeededAmount", newNeededAmount),
                    new SqlParameter("@UserId", requestModel.UserId)
                };
                neededAmountUpdateResult = _service
                    .Execute(conn, transaction, neededAmountUpdateQuery, neededAmountUpdateParams.ToArray());

                #endregion
            }

            #endregion

            if ((normalUpdateBalanceResult > 0 || neededAmountUpdateResult > 0) && result > 0)
            {
                transaction.Commit();
                return StatusCode(201, "Expense Created!");
            }

            transaction.Rollback();
            conn.Close();

            return BadRequest("Creating Fail!");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpPut]
    [Route("/api/expense/{id}")]
    public IActionResult UpdateExpense([FromBody] UpdateExpenseRequestModel requestModel, long id)
    {
        try
        {
            if (requestModel.ExpenseCategoryId <= 0 || requestModel.Amount <= 0 || id == 0 || requestModel.UserId <= 0)
                return BadRequest();

            #region Check Expense Category is valid

            string checkExpenseCategoryActiveQuery = ExpenseCategoryQuery.GetCheckExpenseCategoryActiveQuery();
            List<SqlParameter> checkExpenseCategoryActiveParams = new()
            {
                new SqlParameter("@ExpenseCategoryId", requestModel.ExpenseCategoryId),
                new SqlParameter("@IsActive", true)
            };
            DataTable expenseCategory = _service.QueryFirstOrDefault(checkExpenseCategoryActiveQuery, checkExpenseCategoryActiveParams.ToArray());
            if (expenseCategory.Rows.Count <= 0)
                return NotFound("Expense Category Not Found or Inactive");

            #endregion

            string query = ExpenseQuery.UpdateExpenseQuery();
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@ExpenseId", id),
                new SqlParameter("@UserId", requestModel.UserId),
                new SqlParameter("@ExpenseCategoryId", requestModel.ExpenseCategoryId),
                new SqlParameter("@Amount", requestModel.Amount),
                new SqlParameter("@IsActive", requestModel.Amount)
            };
            int result = _service.Execute(query, parameters.ToArray());

            return result > 0 ? StatusCode(202, "Expense Updated!") : BadRequest("Updating Fail!");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpDelete]
    [Route("/api/expense/{id}")]
    public IActionResult DeleteExpense(long id)
    {
        try
        {
            if (id <= 0)
                return BadRequest();

            string query = ExpenseQuery.DeleteExpenseQuery();
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@IsActive", false),
                new SqlParameter("@ExpenseId", id)
            };
            int result = _service.Execute(query, parameters.ToArray());

            return result > 0 ? StatusCode(202, "Income Deleted!") : BadRequest("Deleting Fail!");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}