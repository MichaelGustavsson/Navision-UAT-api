using Microsoft.EntityFrameworkCore;
using navision.api.Data;
using navision.api.Interfaces;
using navision.api.Models;

namespace navision.api.Repositories
{
  public class CustomerRepository : ICustomerRepository
  {
    private readonly DataContext _context;

    public CustomerRepository(DataContext context){
      _context = context;
    }
    public Task AddCustomer(Customer customer)
    {
      throw new NotImplementedException();
    }

    public async Task<List<Customer>> GetCustomers()
    {
      try
      {
          if(_context is not null){
              return await _context.Customers!.ToListAsync();
          }
          return new();
      }
      catch (Exception ex)
      {
          throw new Exception(ex.Message);
      }
    }
  }
}