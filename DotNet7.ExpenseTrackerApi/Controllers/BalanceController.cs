using DotNet7.ExpenseTrackerApi.Models.RequestModels.Balance;
using DotNet7.ExpenseTrackerApi.Queries;
using DotNet7.ExpenseTrackerApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace DotNet7.ExpenseTrackerApi.Controllers;

public class BalanceController : ControllerBase
{
    private readonly AdoDotNetService _service;

    public BalanceController(AdoDotNetService service)
    {
        _service = service;
    }

    [HttpPut]
    [Route("/api/balance")]
    public IActionResult UpdateBalance([FromBody] BalanceRequestModel requestModel)
    {
        try
        {
            if (requestModel.UserId <= 0)
                return BadRequest("User ID cannot be empty.");

            if (requestModel.Amount <= 0)
                return BadRequest("Amount cannot be empty.");

            string checkUserQuery = UserQuery.CheckUserEixstsQuery();
            List<SqlParameter> checkUserParams = new()
            {
                new SqlParameter("@UserId", requestModel.UserId),
                new SqlParameter("@IsActive", true)
            };
            DataTable user = _service.QueryFirstOrDefault(checkUserQuery, checkUserParams.ToArray());
            if (user.Rows.Count == 0)
                return NotFound("User not found.");

            string balanceUpdateQuery = BalanceQuery.UpdateBalanceQuery();
            List<SqlParameter> parameters = new()
            {
                new SqlParameter("@UserId", requestModel.UserId),
                new SqlParameter("@Amount", requestModel.Amount),
                new SqlParameter("@UpdateDate", DateTime.Now)
            };
            int result = _service.Execute(balanceUpdateQuery, parameters.ToArray());

            return result > 0 ? StatusCode(202, "Balance Updated.") : BadRequest("Updating Fail.");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
