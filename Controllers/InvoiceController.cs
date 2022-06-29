using Microsoft.AspNetCore.Mvc;
using navision.api.Helpers;
using navision.api.Interfaces;
using navision.api.Models;

namespace navision.api.Controllers
{
  [Route("api/invoices")]
  [ApiController]
  public class InvoiceController : ControllerBase
  {
    private readonly IIntegrationRepository _repo;
    private readonly IInternalCarsRepository _iRepo;
    private readonly INaviProRepository _nRepo;

    public InvoiceController(IIntegrationRepository repo,
        INaviProRepository nRepo, IInternalCarsRepository iRepo)
    {
      _nRepo = nRepo;
      _iRepo = iRepo;
      _repo = repo;
    }

    [HttpGet("List/{customerNumber}/{startDate}/{endDate}")]
    public async Task<IActionResult> GetInvoices(string customerNumber, string startDate, string endDate, [FromQuery] InvoiceParams invoiceParams)
    {
      try
      {
        var invoices = await _nRepo.GetInvoices(customerNumber, startDate, endDate, invoiceParams);
        // var invoices = await _nRepo.GetInvoices(customerNumber, startDate, endDate);

        Response.AppPagination(invoices.CurrentPage, invoices.PageSize, invoices.TotalCount, invoices.TotalPages);

        return Ok(invoices);
      }
      catch (Exception ex)
      {
        var errorMessage = System.Text.Json.JsonSerializer.Serialize(ex.Message);
        return BadRequest(errorMessage);
      }
    }

    [HttpGet("FindByInvoiceNumber/{customerNumber}/{invoiceNumber}")]
    public async Task<IActionResult> FindByInvoiceNumber(string customerNumber, string invoiceNumber)
    {
      try
      {
        var invoices = await _nRepo.GetInvoices(customerNumber, invoiceNumber);

        return Ok(invoices);
      }
      catch (Exception ex)
      {
        var errorMessage = System.Text.Json.JsonSerializer.Serialize(ex.Message);
        return BadRequest(errorMessage);
      }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadInvoices(IList<SalesInvoice> model)
    {
      try
      {
        foreach (var invoice in model)
        {
          //Check for the invoice to see if it is already uploaded.
          var result = await _repo.CheckIfExists(invoice.InvoiceNumber);

          if (!result)
          {
            var uploaded = await _iRepo.UploadInvoice(invoice);
            if (!uploaded)
            {
              return BadRequest("Something went wrong when uploading invoices");
            }
          }
        }

        return StatusCode(201);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
  }
}