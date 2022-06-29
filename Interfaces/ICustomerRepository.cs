using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using navision.api.Models;

namespace navision.api.Interfaces
{
    public interface ICustomerRepository
    {
        Task AddCustomer(Customer customer);
        Task<List<Customer>>GetCustomers();
    }
}