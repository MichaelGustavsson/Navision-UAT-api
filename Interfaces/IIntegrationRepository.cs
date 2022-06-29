using System.Threading.Tasks;
using navision.api.Models;

namespace navision.api.Interfaces
{
    public interface IIntegrationRepository
    {
        Task<bool> Add(Invoice invoice);
        Task<bool> CheckIfExists(string invoiceNumber);
    }
}