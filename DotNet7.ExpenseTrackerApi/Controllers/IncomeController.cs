using DotNet7.ExpenseTrackerApi.Models.ResponseModels.Income;
using DotNet7.ExpenseTrackerApi.Queries;
using DotNet7.ExpenseTrackerApi.Services;
using DotNet7.ExpenseTrackerApi.Models.RequestModels.Income;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using DotNet7.ExpenseTrackerApi.Models.Entities;

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
            #region Check Balance according to the User ID

            var balance = await _appDbContext.Balance
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == requestModel.UserId);
            if (balance is null)
                return NotFound("Balance not found.");

            #endregion

            #region Check Income Category Valid

            var incomeCategory = await _appDbContext.IncomeCategory
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IncomeCategoryId == requestModel.IncomeCategoryId && x.IsActive);
            if (incomeCategory is null)
                return NotFound("Income Category Not Found or Inactive.");

            #endregion

            #region Check User Valid

            var user = await _appDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == requestModel.UserId && x.IsActive);
            if (user is null)
                return NotFound("User Not Found or Inactive");

            #endregion

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
                return StatusCode(201, "Creating Successful.");
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
                return NotFound("Income Not Found.");

            var incomeCategory = await _appDbContext.IncomeCategory
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IncomeCategoryId == requestModel.IncomeCategoryId && x.IsActive);
            if (incomeCategory is null)
                return NotFound("Income Category Not Found or Inactive.");

            #region Check User Valid

            var user = await _appDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == requestModel.UserId && x.IsActive);
            if (user is null)
                return NotFound("User Not Found or Inactive");

            #endregion

            #region Check Balance

            var balance = await _appDbContext.Balance
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == requestModel.UserId);
            if (balance is null)
                return NotFound("Balance Not Found.");

            #endregion

            long oldBalance = balance.Amount; // 20000
            long oldIncome = item.Amount; // 10000
            long newIncome = requestModel.Amount; // 5000
            long incomeDifference = 0;

            long newBalance = 0;

            if (newIncome > oldIncome)
            {
                incomeDifference = newIncome - oldIncome;
                newBalance = oldBalance + incomeDifference;
            }
            else
            {
                incomeDifference = oldIncome - newIncome; // 10000 - 5000 = 5000
                newBalance = oldBalance - incomeDifference; // 20000 - 5000 = 15000
            }

            balance.Amount = newBalance;
            _appDbContext.Entry(balance).State = EntityState.Modified;
            int balanceResult = await _appDbContext.SaveChangesAsync();

            item.Amount = newIncome;
            _appDbContext.Entry(item).State = EntityState.Modified;
            int result = await _appDbContext.SaveChangesAsync();

            if (balanceResult > 0 && result > 0)
            {
                await transaction.CommitAsync();
                return StatusCode(202, "Income Updated.");
            }

            await transaction.RollbackAsync();
            return BadRequest("Updating Fail.");
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
