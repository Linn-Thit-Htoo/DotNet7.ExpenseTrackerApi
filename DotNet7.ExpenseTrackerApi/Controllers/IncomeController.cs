using DotNet7.ExpenseTrackerApi.Models.ResponseModels.Income;
using DotNet7.ExpenseTrackerApi.Queries;
using DotNet7.ExpenseTrackerApi.Services;
using DotNet7.ExpenseTrackerApi.Models.RequestModels.Income;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace DotNet7.ExpenseTrackerApi.Controllers;

public class IncomeController : ControllerBase
{
    private readonly AdoDotNetService _adoDotNetService;
    private readonly IConfiguration _configuration;

    public IncomeController(AdoDotNetService adoDotNetService, IConfiguration configuration)
    {
        _adoDotNetService = adoDotNetService;
        _configuration = configuration;
    }

    [HttpGet]
    [Route("/api/income")]
    public IActionResult GetList()
    {
        try
        {
            string query = IncomeQuery.GetIncomeListQuery();
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@IsActive", true)
            };
            List<IncomeResponseModel> lst = _adoDotNetService.Query<IncomeResponseModel>(query, parameters.ToArray());

            return Ok(lst);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet]
    [Route("/api/income/{userID}")]
    public IActionResult GetIncomeListByUserId(long userID)
    {
        try
        {
            if (userID <= 0)
                return BadRequest("User Id cannot be empty.");

            string query = IncomeQuery.GetIncomeListByUserIdQuery();
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@UserId", userID),
                new SqlParameter("@IsActive", true)
            };
            List<IncomeResponseModel> lst = _adoDotNetService.Query<IncomeResponseModel>(query, parameters.ToArray());

            return Ok(lst);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpPost]
    [Route("/api/income")]
    public IActionResult CreateIncome([FromBody] IncomeRequestModel requestModel)
    {
        SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
        conn.Open();
        SqlTransaction transaction = conn.BeginTransaction();

        try
        {
            #region Validation

            if (requestModel.IncomeCategoryId <= 0)
                return BadRequest();

            if (requestModel.Amount <= 0)
                return BadRequest();

            if (requestModel.UserId <= 0)
                return BadRequest();

            if (string.IsNullOrEmpty(requestModel.CreateDate))
                return BadRequest();

            #endregion

            #region Check Income Category is valid

            string checkIncomeCategoryActiveQuery = IncomeCategoryQuery.CheckIncomeCategoryActiveQuery();
            List<SqlParameter> checkIncomeCategoryActiveParams = new()
            {
                new SqlParameter("@IncomeCategoryId", requestModel.IncomeCategoryId),
                new SqlParameter("@IsActive", true)
            };
            DataTable incomeCategory = _adoDotNetService.QueryFirstOrDefault(checkIncomeCategoryActiveQuery, checkIncomeCategoryActiveParams.ToArray());
            if (incomeCategory.Rows.Count == 0)
                return NotFound("Income Category Not Found or Inactive");

            #endregion

            #region Income Create

            string query = IncomeQuery.CreateIncomeQuery();
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@IncomeCategoryId", requestModel.IncomeCategoryId),
                new SqlParameter("@UserId", requestModel.UserId),
                new SqlParameter("@Amount", requestModel.Amount),
                new SqlParameter("@CreateDate", requestModel.CreateDate),
                new SqlParameter("@IsActive", true)
            };
            int result = _adoDotNetService.Execute(conn, transaction, query, parameters.ToArray());

            #endregion

            #region Get Old Money

            // already has money + income money
            string getOldMoneyQuery = @"SELECT [BalanceId]
      ,[UserId]
      ,[Amount]
      ,[CreateDate]
      ,[UpdateDate]
  FROM [dbo].[Balance] WHERE UserId = @UserId";
            List<SqlParameter> getOldMoneyParams = new()
            {
                new SqlParameter("@UserId", requestModel.UserId)
            };
            DataTable oldMoneyDt = _adoDotNetService.QueryFirstOrDefault(getOldMoneyQuery, getOldMoneyParams.ToArray());
            long oldMoney = Convert.ToInt64(oldMoneyDt.Rows[0]["Amount"]);

            #endregion

            long updatedMoney = oldMoney + requestModel.Amount;

            #region Update Balance

            // ဝင်ငွေ ပမာဏ က လိုအပ်ငွေထက် နဲရင်နဲမယ် များရင်များမယ် တူရင် တူမယ်

            #region Get Old Needed Amount Query

            string getOldNeededAmountQuery = @"SELECT TOP (1000) [BalanceId]
      ,[UserId]
      ,[Amount]
      ,[NeededAmount]
      ,[CreateDate]
      ,[UpdateDate]
  FROM [ExpenseTracker].[dbo].[Balance] WHERE UserId = @UserId";
            SqlParameter[] getOldNeededAmountParams = { new("@UserId", requestModel.UserId) };
            DataTable oldBalanceDt = _adoDotNetService
                .QueryFirstOrDefault(conn, transaction, getOldNeededAmountQuery, getOldNeededAmountParams);

            #endregion

            long oldNeededAmount = Convert.ToInt64(oldBalanceDt.Rows[0]["NeededAmount"]);
            long oldBalanceAmount = Convert.ToInt64(oldBalanceDt.Rows[0]["Amount"]);

            int balanceUpdateResult = 0;
            int neededAmountUpdateResult = 0;
            int resetAmountResult = 0;

            #region ဝင်ငွေပမာဏက လိုအပ်ငွေထက် များနေခဲ့ရင်

            if (requestModel.Amount > oldNeededAmount)
            {
                long totalBalance = requestModel.Amount - oldNeededAmount + oldBalanceAmount;
                string updateBalanceQuery = @"UPDATE Balance SET Amount = @Amount, NeededAmount = @NeededAmount
WHERE UserId = @UserId";
                List<SqlParameter> updateBalanceParams = new()
                {
                    new SqlParameter("@Amount", totalBalance),
                    new SqlParameter("@NeededAmount", Convert.ToInt64(0)),
                    new SqlParameter("@UserId", requestModel.UserId)
                };
                balanceUpdateResult = _adoDotNetService
                    .Execute(conn, transaction, updateBalanceQuery, updateBalanceParams.ToArray());

                if (balanceUpdateResult <= 0)
                {
                    transaction.Rollback();
                    return BadRequest("Updating Fail.");
                }
            }

            #endregion

            #region ဝင်ငွေပမာဏက လိုအပ်ငွေထက် နဲနေခဲ့ရင်

            else if (requestModel.Amount < oldNeededAmount)
            {
                long totalAmount = oldNeededAmount - requestModel.Amount;
                string neededAmountUpdateQuery = @"UPDATE Balance SET Amount = @Amount, NeededAmount = @NeededAmount
WHERE UserId = @UserId";
                List<SqlParameter> neededAmountUpdateParams = new()
                {
                    new SqlParameter("@Amount", Convert.ToInt64(0)),
                    new SqlParameter("@NeededAmount", totalAmount),
                    new SqlParameter("@UserId", requestModel.UserId)
                };
                neededAmountUpdateResult = _adoDotNetService
                    .Execute(conn, transaction, neededAmountUpdateQuery, neededAmountUpdateParams.ToArray());

                if (neededAmountUpdateResult <= 0)
                {
                    transaction.Rollback();
                    return BadRequest("Updating Fail.");
                }
            }

            #endregion

            #region ဝင်ငွေပမာဏနဲ့ လိုအပ်ငွေ နဲ့ တူနေခဲ့ရင် (အကြွေးကျေတဲ့case)

            else if (requestModel.Amount == oldNeededAmount)
            {
                string resetAmountQuery = @"UPDATE Balance SET Amount = @Amount, NeededAmount = @NeededAmount
WHERE UserId = @UserId";
                List<SqlParameter> resetAmountParams = new()
                {
                    new SqlParameter("@Amount", Convert.ToInt64(0)),
                    new SqlParameter("@NeededAmount", Convert.ToInt64(0)),
                    new SqlParameter("@UserId", requestModel.UserId)
                };
                resetAmountResult = _adoDotNetService
                    .Execute(conn, transaction, resetAmountQuery, resetAmountParams.ToArray());

                if (resetAmountResult <= 0)
                {
                    transaction.Rollback();
                    return BadRequest("Updating Fail.");
                }
            }

            #endregion

            #endregion

            if (result > 0 && (balanceUpdateResult > 0 || neededAmountUpdateResult > 0 || resetAmountResult > 0))
            {
                transaction.Commit();
                return StatusCode(201, "Income Created!");
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
    [Route("/api/income/{id}")]
    public IActionResult UpdateIncome([FromBody] UpdateIncomeRequestModel requestModel, long id)
    {
        try
        {
            if (requestModel.IncomeCategoryId <= 0 || requestModel.Amount <= 0 || id <= 0 || requestModel.UserId <= 0)
                return BadRequest();

            #region Check Income Category is valid

            string checkIncomeCategoryActiveQuery = IncomeCategoryQuery.CheckIncomeCategoryActiveQuery();
            List<SqlParameter> checkIncomeCategoryActiveParams = new()
            {
                new SqlParameter("@IncomeCategoryId", requestModel.IncomeCategoryId),
                new SqlParameter("@IsActive", true)
            };
            DataTable incomeCategory = _adoDotNetService.QueryFirstOrDefault(checkIncomeCategoryActiveQuery, checkIncomeCategoryActiveParams.ToArray());
            if (incomeCategory.Rows.Count == 0)
                return NotFound("Income Category Not Found or Inactive");

            #endregion

            string query = IncomeQuery.UpdateIncomeQuery();
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@IncomeId", id),
                new SqlParameter("@UserId", requestModel.UserId),
                new SqlParameter("@IncomeCategoryId", requestModel.IncomeCategoryId),
                new SqlParameter("@Amount", requestModel.Amount)
            };
            int result = _adoDotNetService.Execute(query, parameters.ToArray());

            return result > 0 ? StatusCode(202, "Income Updated!") : BadRequest("Updating Fail!");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpDelete]
    [Route("/api/income/{id}")]
    public IActionResult DeleteIncome(long id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Id cannot be empty.");

            string query = IncomeQuery.DeleteIncomeQuery();
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@IsActive", false),
                new SqlParameter("@IncomeId", id)
            };
            int result = _adoDotNetService.Execute(query, parameters.ToArray());

            return result > 0 ? StatusCode(202, "Income Deleted!") : BadRequest("Deleting Fail!");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
