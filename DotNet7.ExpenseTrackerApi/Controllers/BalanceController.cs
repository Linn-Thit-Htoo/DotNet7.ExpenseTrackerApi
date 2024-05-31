using DotNet7.ExpenseTrackerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNet7.ExpenseTrackerApi.Controllers;

public class BalanceController : ControllerBase
{
    private readonly AdoDotNetService _service;

    public BalanceController(AdoDotNetService service)
    {
        _service = service;
    }
}