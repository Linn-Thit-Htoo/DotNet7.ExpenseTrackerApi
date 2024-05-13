using DotNet7.ExpenseTrackerApi.Models.RequestModels.Expense;
using DotNet7.ExpenseTrackerApi.Models.ResponseModels.Expense;
using DotNet7.ExpenseTrackerApi.Services;
using DotNet7.ExpenseTrackerApi.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace DotNet7.ExpenseTrackerApi.Controllers;

public class ExpenseController : ControllerBase
{
    private readonly AdoDotNetService _service;

    public ExpenseController(AdoDotNetService service)
    {
        _service = service;
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
        try
        {
            if (requestModel.ExpenseCategoryId <= 0)
                return BadRequest();

            if (requestModel.Amount <= 0)
                return BadRequest();

            if (requestModel.UserId <= 0)
                return BadRequest();

            if (string.IsNullOrEmpty(requestModel.CreateDate))
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

            string query = ExpenseQuery.CreateExpenseQuery();
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@ExpenseCategoryId", requestModel.ExpenseCategoryId),
                new SqlParameter("@UserId", requestModel.UserId),
                new SqlParameter("@Amount", requestModel.Amount),
                new SqlParameter("@CreateDate", requestModel.CreateDate),
                new SqlParameter("@IsActive", true),
            };
            int result = _service.Execute(query, parameters.ToArray());

            return result > 0 ? StatusCode(201, "Expense Created!") : BadRequest("Creating Fail!");
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