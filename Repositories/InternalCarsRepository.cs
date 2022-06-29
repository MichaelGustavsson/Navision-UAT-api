using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using navision.api.Helpers;
using navision.api.Interfaces;
using navision.api.Models;

namespace navision.api.Repositories
{
  public class InternalCarsRepository : IInternalCarsRepository
  {
    private readonly IConfiguration _configuration;
    private readonly IIntegrationRepository _repo;
    private Token? _token;
    private string _json = string.Empty;
    private string _checkSum = string.Empty;


    public InternalCarsRepository(IConfiguration configuration, IIntegrationRepository repo)
    {
      _configuration = configuration;
      _repo = repo;
    }

    public async Task<bool> UploadInvoice(SalesInvoice invoice)
    {

      var url = _configuration["InternalCarsSettings:invoiceUrl"];

      if (_token == null) { _token = await GetToken(); }

      var data = DeserializeData.MapToInternalCars(invoice);

      // var json = JsonConvert.SerializeObject(data);
      var json = JsonSerializer.Serialize(data);
      var checkSum = AdlerStandaloneChecksum.ComputeAdlerChecksum(json);

      using (var client = new HttpClient())
      {

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token.Access_Token);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var postContent = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{url}?adlerChecksum={checkSum}", postContent);

        if (response.IsSuccessStatusCode)
        {
          var message = response.Content.ReadAsStringAsync().Result;
          await _repo.Add(new Invoice { InvoiceNumber = invoice.InvoiceNumber, UploadDate = DateTime.Now, Data = json, Message = message });
          return true;
        }
        else
        {
          var message = response.Content.ReadAsStringAsync().Result;
          await _repo.Add(new Invoice { InvoiceNumber = invoice.InvoiceNumber, UploadDate = DateTime.Now, Data = json, Message = message });
          throw new Exception(message);
        }
      }
    }

    private async Task<Token> GetToken()
    {
      var options = new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      };

      try
      {
        var url = _configuration["InternalCarsSettings:tokenUrl"];
        var clientId = _configuration["InternalCarsSettings:client_id"];
        var scope = _configuration["InternalCarsSettings:scope"];
        var clientSecret = _configuration["InternalCarsSettings:client_secret"];
        var address = _configuration["InternalCarsSettings:address"];
        var grantType = _configuration["InternalCarsSettings:grant_type"];

        var handler = new HttpClientHandler
        {
          ServerCertificateCustomValidationCallback = (requestMessage, certificate, chain, policyErrors) => true
        };

        using (var client = new HttpClient(handler))
        {
          using (var request = new HttpRequestMessage(new HttpMethod("POST"), url))
          {
            request.Content = new StringContent($"client_id={clientId}&scope={scope}&client_secret={clientSecret}&address={address}&grant_type={grantType}", Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
              var content = response.Content;
              var result = await content.ReadAsStringAsync();
              var json = JsonSerializer.Deserialize<Token>(result, options);
              return json!;
            }
            else
            {
              throw new Exception("Couldn't get a bearer token");
            }
          }
        }
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
  }

  public class AdlerStandaloneChecksum
  {
    /// <summary>
    /// AdlerBase is Adler-32 checksum algorithm parameter.
    /// </summary>
    private const uint AdlerBase = 0xFFF1;
    /// AdlerStart is Adler-32 checksum algorithm parameter.
    /// </summary>
    private const uint AdlerStart = 0x0001;
    /// <summary>
    /// AdlerBuff is Adler-32 checksum algorithm parameter.
    /// </summary>
    private const uint AdlerBuff = 0x0400;

    private static uint ComputeAdlerChecksumForBuff(byte[] bytesBuff)
    {
      if (Object.Equals(bytesBuff, null))
      {
        return 0;
      }
      int nSize = bytesBuff.GetLength(0);
      if (nSize == 0)
      {
        return 0;
      }
      uint unSum1 = AdlerStart & 0xFFFF;
      uint unSum2 = (AdlerStart >> 16) & 0xFFFF;
      for (int i = 0; i < nSize; i++)
      {
        unSum1 = (unSum1 + bytesBuff[i]) % AdlerBase;
        unSum2 = (unSum1 + unSum2) % AdlerBase;
      }
      return (unSum2 << 16) + unSum1;
    }

    public static string ComputeAdlerChecksum(string encodedString)
    {
      string ticketSeed = encodedString;
      byte[] ticketBytes = Encoding.ASCII.GetBytes(ticketSeed);
      return ComputeAdlerChecksumForBuff(ticketBytes).ToString();
    }

  }
}