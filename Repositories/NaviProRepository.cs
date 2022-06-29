using System.ServiceModel;
using navision.api.Data;
using navision.api.Helpers;
using navision.api.Interfaces;
using navision.api.Models;
using NavisionService;

namespace navision.api.Repositories
{
  public class NaviProRepository : INaviProRepository
  {
    /*
    <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:bca="urn:microsoft-dynamics-schemas/codeunit/BCA_Web_Service">
        <soapenv:Header/>
        <soapenv:Body>
            <bca:PerformService>
                <bca:serviceName>GetSalesCreditMemos</bca:serviceName>
                <bca:xMLData><![CDATA[<GetSalesCreditMemos><CustomerNo>AV5990</CustomerNo><StartingDate>2012-10-12</StartingDate><EndingDate>2020-03-27</EndingDate><InvoiceNo>FKR1700459</InvoiceNo></GetSalesCreditMemos>]]></bca:xMLData>
            </bca:PerformService>
        </soapenv:Body>
    </soapenv:Envelope>
    */
    private readonly IConfiguration _configuration;
    private readonly IIntegrationRepository _repo;
    private readonly DataContext _context;

    public NaviProRepository(IConfiguration configuration, IIntegrationRepository repo, DataContext context)
    {
      _context = context;
      _repo = repo;
      _configuration = configuration;
    }
    public async Task<PagedList<SalesInvoice>> GetInvoices(string customerNo, string startDate, string endDate, InvoiceParams invoiceParams)
    {
      try
      {
        var client = CreateConnection();
        client.ClientCredentials.UserName.UserName = _configuration["NaviProSettings:userName"];
        client.ClientCredentials.UserName.Password = _configuration["NaviProSettings:password"];

        // Get debit invoices
        var request = $"<GetSalesInvoices><CustomerNo>{customerNo}</CustomerNo><StartingDate>{startDate}</StartingDate><EndingDate>{endDate}</EndingDate></GetSalesInvoices>";
        var result = await client.PerformServiceAsync("GetSalesInvoices", request);

        if (!result.return_value.StartsWith("<"))
        {
          throw new Exception(result.return_value);
        }

        var invoiceList = DeserializeData.Deserialize(result.return_value);

        // Get credit invoices
        request = $"<GetSalesCreditMemos><CustomerNo>{customerNo}</CustomerNo><StartingDate>{startDate}</StartingDate><EndingDate>{endDate}</EndingDate></GetSalesCreditMemos>";
        result = await client.PerformServiceAsync("GetSalesCreditMemos", request);

        if (!result.return_value.StartsWith("<"))
        {
          throw new Exception(result.return_value);
        }

        var creditNoteList = DeserializeData.Deserialize(result.return_value);

        var list = await GenerateResult(invoiceList, creditNoteList, invoiceParams);

        return list;
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<List<SalesInvoice>> GetInvoices(string customerNo, string startDate, string endDate)
    {
      try
      {
        var client = CreateConnection();
        client.ClientCredentials.UserName.UserName = _configuration["NaviProSettings:userName"];
        client.ClientCredentials.UserName.Password = _configuration["NaviProSettings:password"];

        // Get debit invoices
        var request = $"<GetSalesInvoices><CustomerNo>{customerNo}</CustomerNo><StartingDate>{startDate}</StartingDate><EndingDate>{endDate}</EndingDate></GetSalesInvoices>";
        var result = await client.PerformServiceAsync("GetSalesInvoices", request);

        if (!result.return_value.StartsWith("<"))
        {
          throw new Exception(result.return_value);
        }

        var invoiceList = DeserializeData.Deserialize(result.return_value);

        // Get credit invoices
        request = $"<GetSalesCreditMemos><CustomerNo>{customerNo}</CustomerNo><StartingDate>{startDate}</StartingDate><EndingDate>{endDate}</EndingDate></GetSalesCreditMemos>";
        result = await client.PerformServiceAsync("GetSalesCreditMemos", request);

        if (!result.return_value.StartsWith("<"))
        {
          throw new Exception(result.return_value);
        }

        var creditNoteList = DeserializeData.Deserialize(result.return_value);

        var list = await GenerateResult(invoiceList, creditNoteList);

        return list;
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
    public async Task<List<SalesInvoice>> GetInvoices(string customerNo, string invoiceNumber)
    {
      try
      {
        string startDate = DateTime.MinValue.ToString("yyyy-MM-dd");
        string endDate = DateTime.MaxValue.ToString("yyyy-MM-dd");

        var client = CreateConnection();
        client.ClientCredentials.UserName.UserName = _configuration["NaviProSettings:userName"];
        client.ClientCredentials.UserName.Password = _configuration["NaviProSettings:password"];

        // Get debit invoices
        var request = $"<GetSalesInvoices><CustomerNo>{customerNo}</CustomerNo><StartingDate>{startDate}</StartingDate><EndingDate>{endDate}</EndingDate><InvoiceNo>{invoiceNumber}</InvoiceNo></GetSalesInvoices>";
        var result = await client.PerformServiceAsync("GetSalesInvoices", request);

        if (!result.return_value.StartsWith("<"))
        {
          throw new Exception(result.return_value);
        }
        var invoiceList = DeserializeData.Deserialize(result.return_value);

        // Get credit invoices
        request = $"<GetSalesCreditMemos><CustomerNo>{customerNo}</CustomerNo><StartingDate>{startDate}</StartingDate><EndingDate>{endDate}</EndingDate><InvoiceNo>{invoiceNumber}</InvoiceNo></GetSalesCreditMemos>";
        result = await client.PerformServiceAsync("GetSalesCreditMemos", request);

        if (!result.return_value.StartsWith("<"))
        {
          throw new Exception(result.return_value);
        }

        var creditNoteList = DeserializeData.Deserialize(result.return_value);

        var list = await GenerateResult(invoiceList, creditNoteList);

        return list;
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    private BCA_Web_Service_PortClient CreateConnection()
    {
      EndpointAddress url = new EndpointAddress(_configuration["NaviProSettings:endpoint"]);
      BasicHttpBinding binding = new BasicHttpBinding();
      binding.Security.Mode = BasicHttpSecurityMode.Transport;
      binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
      binding.MaxReceivedMessageSize = 52428800;

      BCA_Web_Service_PortClient client = new BCA_Web_Service_PortClient(binding, url);
      client.ClientCredentials.UserName.UserName = _configuration["NaviProSettings:userName"];
      client.ClientCredentials.UserName.Password = _configuration["NaviProSettings:password"];

      return client;
    }

    private async Task<List<SalesInvoice>> GenerateResult(SalesInvoices invoices, SalesInvoices creditNotes)
    {
      try
      {
        var list = new List<SalesInvoice>();
        var tmpList = new List<SalesInvoice>(invoices.SalesInvoiceList!.Count + creditNotes.SalesInvoiceList!.Count);

        tmpList.AddRange(invoices.SalesInvoiceList);
        tmpList.AddRange(creditNotes.SalesInvoiceList);
        tmpList.Sort((x, y) => x.InvoiceDate.CompareTo(y.InvoiceDate));

        foreach (var invoice in tmpList)
        {
          var exists = await _repo.CheckIfExists(invoice.InvoiceNumber);
          if (!exists)
          {
            list.Add(invoice);
          }
        }

        return list;
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        throw new Exception(e.Message);
      }
    }

    private async Task<PagedList<SalesInvoice>> GenerateResult(SalesInvoices invoices, SalesInvoices creditNotes, InvoiceParams invoiceParams)
    {
      try
      {
        var list = new List<SalesInvoice>();
        var tmpList = new List<SalesInvoice>(invoices.SalesInvoiceList!.Count + creditNotes.SalesInvoiceList!.Count);

        tmpList.AddRange(invoices.SalesInvoiceList);
        tmpList.AddRange(creditNotes.SalesInvoiceList);
        tmpList.Sort((x, y) => x.InvoiceDate.CompareTo(y.InvoiceDate));

        foreach (var invoice in tmpList)
        {
          var exists = await _repo.CheckIfExists(invoice.InvoiceNumber);
          if (!exists)
          {
            list.Add(invoice);
          }
        }

        return PagedList<SalesInvoice>.Create(list, invoiceParams.PageNumber, invoiceParams.PageSize);
        // return PagedList<SalesInvoice>.CreateAsync(tmpList, invoiceParams.PageNumber, invoiceParams.PageSize);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        throw new Exception(e.Message);
      }
    }
  }
}