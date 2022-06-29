using System.Threading.Tasks;
using navision.api.Models;

namespace navision.api.Interfaces
{
    public interface IInternalCarsRepository
    {
         Task<bool> UploadInvoice(SalesInvoice invoice);
    }
}