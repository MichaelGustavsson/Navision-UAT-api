using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace navision.api.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string CustomerNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
    }
}