using Microsoft.EntityFrameworkCore;
using navision.api.Data;
using navision.api.Interfaces;
using navision.api.Models;

namespace navision.api.Repositories
{
  public class IntegrationRepository: IIntegrationRepository
    {
        private readonly DataContext _context;

        public IntegrationRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> Add(Invoice invoice)
        {
            try
            {
                _context.Invoices!.Add(invoice);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CheckIfExists(string invoiceNumber)
        {
            try
            {                  
                if(_context is not null){
                    if(_context.Invoices is not null){
                        var result = await _context.Invoices.Where(c => c.InvoiceNumber.Trim().ToLower() == invoiceNumber.Trim().ToLower()).FirstOrDefaultAsync();
                        if(result == null)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}