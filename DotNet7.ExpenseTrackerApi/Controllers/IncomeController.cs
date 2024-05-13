using DotNet7.ExpenseTrackerApi.Models.ResponseModels.Income;
using DotNet7.ExpenseTrackerApi.Queries;
using DotNet7.ExpenseTrackerApi.Services;
using DotNet7.ExpenseTrackerApi.Models.RequestModels.Income;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Transactions;
using System.Diagnostics.Eventing.Reader;
using Microsoft.EntityFrameworkCore;
using DotNet7.ExpenseTrackerApi.Models.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DotNet7.ExpenseTrackerApi.Controllers;

public class IncomeController : ControllerBase
{
    private readonly AdoDotNetService _adoDotNetService;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _appDbContext;

    public IncomeController(AdoDotNetService adoDotNetService, IConfiguration configuration, AppDbContext appDbContext)
    {
        _adoDotNetService = adoDotNetService;
        _configuration = configuration;
        _appDbContext = appDbContext;
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
    public async Task<IActionResult> CreateIncome([FromBody] IncomeRequestModel requestModel)
    {
        var transaction = _appDbContext.Database.BeginTransaction();
        try
        {
            var balance = await _appDbContext.Balance
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == requestModel.UserId);
            if (balance is null)
                return NotFound();

            long updatedBalance = balance.Amount + requestModel.Amount;

            balance.Amount = updatedBalance;
            _appDbContext.Entry(balance).State = EntityState.Modified;
            int balanceResult = await _appDbContext.SaveChangesAsync();

            IncomeModel model = new()
            {
                Amount = requestModel.Amount,
                CreateDate = requestModel.CreateDate,
                IncomeCategoryId = requestModel.IncomeCategoryId,
                IsActive = true,
                UserId = requestModel.UserId
            };
            await _appDbContext.Income.AddAsync(model);
            int incomeResult = await _appDbContext.SaveChangesAsync();

            if (balanceResult > 0 && incomeResult > 0)
            {
                transaction.Commit();
                return StatusCode(201, "Creating Successful");
            }

            transaction.Rollback();
            return BadRequest("Creating Fail.");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new Exception(ex.Message);
        }
    }

    [HttpPut]
    [Route("/api/income/{id}")]
    public async Task<IActionResult> UpdateIncome([FromBody] UpdateIncomeRequestModel requestModel, long id)
    {
        var transaction = _appDbContext.Database.BeginTransaction();
        try
        {
            var item = await _appDbContext.Income
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IncomeId == id && x.IsActive);

            if (item is null)
                return NotFound();

            item.IncomeCategoryId = requestModel.IncomeCategoryId;
            _appDbContext.Entry(item).State = EntityState.Modified;
            int incomeUpdateResult = await _appDbContext.SaveChangesAsync();

            var balance = await _appDbContext.Balance
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == requestModel.UserId);

            if (balance is null)
                return NotFound();

            long updatedBalance = balance.Amount + requestModel.Amount;
            balance.Amount = updatedBalance;
            _appDbContext.Entry(balance).State = EntityState.Modified;
            int balanceUpdateResult = await _appDbContext.SaveChangesAsync();

            if (incomeUpdateResult > 0 && balanceUpdateResult > 0)
            {
                transaction.Commit();
                return StatusCode(202, "Updating Successful");
            }

            transaction.Rollback();
            return BadRequest("Creating Fail.");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
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
