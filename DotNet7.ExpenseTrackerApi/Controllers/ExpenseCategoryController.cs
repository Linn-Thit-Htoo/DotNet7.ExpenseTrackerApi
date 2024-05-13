using DotNet7.ExpenseTrackerApi.Models.Entities;
using DotNet7.ExpenseTrackerApi.Models.RequestModels.ExpenseCategory;
using DotNet7.ExpenseTrackerApi.Queries;
using DotNet7.ExpenseTrackerApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace DotNet7.ExpenseTrackerApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExpenseCategoryController : ControllerBase
{
    private readonly AdoDotNetService _service;

    public ExpenseCategoryController(AdoDotNetService service)
    {
        _service = service;
    }

    [HttpGet]
    [Route("/api/expense-category")]
    public IActionResult GetList()
    {
        try
        {
            string query = ExpenseCategoryQuery.GetExpenseCategoryListQuery();
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@IsActive", true)
            };
            List<ExpenseCategoryModel> lst = _service.Query<ExpenseCategoryModel>(query, parameters.ToArray());

            return Ok(lst);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpPost]
    [Route("/api/expense-category")]
    public IActionResult CreateExpenseCategory([FromBody] ExpenseCategoryRequestModel requestModel)
    {
        try
        {
            if (string.IsNullOrEmpty(requestModel.ExpenseCategoryName))
                return BadRequest("Category name cannot be empty.");

            string duplicateQuery = ExpenseCategoryQuery.CheckCreateExpenseCategoryDuplicateQuery();
            List<SqlParameter> duplicateParams = new()
            {
                new SqlParameter("@ExpenseCategoryName", requestModel.ExpenseCategoryName),
                new SqlParameter("@IsActive", true)
            };
            DataTable dt = _service.QueryFirstOrDefault(duplicateQuery, duplicateParams.ToArray());
            if (dt.Rows.Count > 0)
                return Conflict("Expense Category Name already exists!");

            string query = ExpenseCategoryQuery.CreateExpenseCategoryQuery();
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@ExpenseCategoryName", requestModel.ExpenseCategoryName),
                new SqlParameter("@IsActive", true)
            };
            int result = _service.Execute(query, parameters.ToArray());

            return result > 0 ? StatusCode(201, "Creating Successful!") : BadRequest("Creating Fail!");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    //[HttpPut]
    //[Route("/api/expense-category/{id}")]
    //public IActionResult UpdateExpenseCategory([FromBody] ExpenseCategoryRequestModel requestModel, long id)
    //{
    //    try
    //    {
    //        if (string.IsNullOrEmpty(requestModel.ExpenseCategoryName))
    //            return BadRequest("Category name cannot be empty.");

    //        string duplicateQuery = ExpenseCategoryQuery.CheckUpdateExpenseCategoryDuplicateQuery();
    //        List<SqlParameter> duplicateParams = new()
    //        {
    //            new SqlParameter("@ExpenseCategoryName", requestModel.ExpenseCategoryName),
    //            new SqlParameter("@IsActive", true),
    //            new SqlParameter("@ExpenseCategoryId", id)
    //        };
    //        DataTable dt = _service.QueryFirstOrDefault(duplicateQuery, duplicateParams.ToArray());
    //        if (dt.Rows.Count > 0)
    //            return Conflict("Expense Category Name already exists.");

    //        string query = ExpenseCategoryQuery.UpdateExpenseCategoryQuery();
    //        List<SqlParameter> parameters = new()
    //        {
    //            new SqlParameter("@ExpenseCategoryName", requestModel.ExpenseCategoryName),
    //            new SqlParameter("@ExpenseCategoryId", id)
    //        };
    //        int result = _service.Execute(query, parameters.ToArray());

    //        return result > 0 ? StatusCode(202, "Updating Successful!") : BadRequest("Updating Fail!");
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new Exception(ex.Message);
    //    }
    //}

    // delete
    [HttpDelete]
    [Route("/api/expense-category/{id}")]
    public IActionResult DeleteExpenseCategory(long id)
    {
        try
        {
            string validateQuery = ExpenseCategoryQuery.CheckExpenseCategoryQuery();
            List<SqlParameter> validateParams = new()
            {
                new SqlParameter("@ExpenseCategoryId", id)
            };
            DataTable dt = _service.QueryFirstOrDefault(validateQuery, validateParams.ToArray());

            if (dt.Rows.Count > 0)
                return Conflict("Expense with this category already exists! Cannot delete.");

            string query = ExpenseCategoryQuery.DeleteExpenseCategoryQuery();
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@ExpenseCategoryId", id),
                new SqlParameter("@IsActive", false)
            };
            int result = _service.Execute(query, parameters.ToArray());

            return result > 0 ? StatusCode(202, "Deleting Successful!") : BadRequest("Deleting Fail!");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
