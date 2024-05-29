using DotNet7.ExpenseTrackerApi.Models.Entities;
using DotNet7.ExpenseTrackerApi.Models.RequestModels.IncomeCategory;
using DotNet7.ExpenseTrackerApi.Services;
using DotNet7.ExpenseTrackerApi.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace DotNet7.ExpenseTrackerApi.Controllers;

public class IncomeCategoryController : ControllerBase
{
    private readonly AdoDotNetService _service;

    public IncomeCategoryController(AdoDotNetService service)
    {
        _service = service;
    }

    [HttpGet]
    [Route("/api/income-category")]
    public IActionResult GetIncomeCategory()
    {
        try
        {
            string query = IncomeCategoryQuery.GetIncomeCategoryListQuery();
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@IsActive", true)
            };
            List<IncomeCategoryModel> lst = _service.Query<IncomeCategoryModel>(query, parameters.ToArray());

            return Ok(lst);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpPost]
    [Route("/api/income-category")]
    public IActionResult CreateIncomeCategory([FromBody] IncomeCategoryRequestModel requestModel)
    {
        try
        {
            if (string.IsNullOrEmpty(requestModel.IncomeCategoryName))
                return BadRequest("Category Name cannot be empty.");

            string duplicateQuery = IncomeCategoryQuery.CheckCreateIncomeCategoryDuplicateQuery();
            List<SqlParameter> duplicateParams = new()
            {
                new SqlParameter("@IncomeCategoryName", requestModel.IncomeCategoryName),
                new SqlParameter("@IsActive", true)
            };
            DataTable category = _service.QueryFirstOrDefault(duplicateQuery, duplicateParams.ToArray());
            if (category.Rows.Count > 0)
                return Conflict("Income Category Name already exists.");

            string query = IncomeCategoryQuery.CreateIncomeCategoryQuery();
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@IncomeCategoryName", requestModel.IncomeCategoryName),
                new SqlParameter("@IsActive", true)
            };
            int result = _service.Execute(query, parameters.ToArray());

            return result > 0 ? StatusCode(201, "Income Category Created.") : BadRequest("Creating Fail.");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    //[HttpPut]
    //[Route("/api/income-category/{id}")]
    //public IActionResult UpdateIncomeCategory([FromBody] IncomeCategoryRequestModel requestModel, long id)
    //{
    //    try
    //    {
    //        if (string.IsNullOrEmpty(requestModel.IncomeCategoryName))
    //            return BadRequest("Category name cannot be empty.");

    //        string duplicateQuery = IncomeCategoryQuery.CheckIncomeCategoryDuplicateQuery();
    //        List<SqlParameter> duplicateParams = new()
    //        {
    //            new SqlParameter("@IncomeCategoryName", requestModel.IncomeCategoryName),
    //            new SqlParameter("@IsActive", true),
    //            new SqlParameter("@IncomeCategoryId", id)
    //        };
    //        DataTable dt = _service.QueryFirstOrDefault(duplicateQuery, duplicateParams.ToArray());
    //        if (dt.Rows.Count > 0)
    //            return Conflict("Income Category Name already exists.");

    //        string query = IncomeCategoryQuery.UpdateIncomeCategoryQuery();
    //        List<SqlParameter> parameters = new()
    //        {
    //            new SqlParameter("@IncomeCategoryName", requestModel.IncomeCategoryName),
    //            new SqlParameter("@IncomeCategoryId", id)
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
    [Route("/api/income-category/{id}")]
    public IActionResult DeleteIncomeCategory(long id)
    {
        try
        {
            if (id == 0)
                return BadRequest();

            string validateQuery = IncomeQuery.GetCheckIncomeExistsQuery();
            SqlParameter[] validateParams = { new("@IncomeCategoryId", id) };
            DataTable dt = _service.QueryFirstOrDefault(validateQuery, validateParams);

            if (dt.Rows.Count > 0)
                return Conflict("Income with this category already exists! Cannot delete.");

            string query = IncomeCategoryQuery.DeleteIncomeCategoryQuery();
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@IsActive", false),
                new SqlParameter("@IncomeCategoryId", id)
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