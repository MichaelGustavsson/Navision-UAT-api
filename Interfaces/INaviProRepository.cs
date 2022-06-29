using System.Collections.Generic;
using System.Threading.Tasks;
using navision.api.Helpers;
using navision.api.Models;

namespace navision.api.Interfaces
{
    public interface INaviProRepository
    {
        Task<PagedList<SalesInvoice>> GetInvoices(string customerNo, string startDate, string endDate, InvoiceParams invoiceParams);
        Task<List<SalesInvoice>> GetInvoices(string customerNo, string startDate, string endDate);
        Task<List<SalesInvoice>> GetInvoices(string customerNo, string invoiceNumber);
    }
}