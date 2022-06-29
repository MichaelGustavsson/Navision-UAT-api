using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using navision.api.Interfaces;

namespace navision.api.Controllers
{
  [ApiController]
  [Route("api/customers")]
  public class CustomerController : ControllerBase
  {
    private readonly ICustomerRepository _repo;

    public CustomerController(ICustomerRepository repo)
    {
      _repo = repo;
    }

    [HttpGet()]
    public async Task<IActionResult> GetCustomers()
    {
      try
      {
        var customers = await _repo.GetCustomers();
        return Ok(customers);
      }
      catch (Exception ex)
      {
        return NotFound($"Couldn't find any customers du to a problem: {ex.Message}");
      }
    }
  }
}