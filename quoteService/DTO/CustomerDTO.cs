using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace quoteService.DTO
{
    public class CustomerDTO
    {
        public int CustomerId { get; set; }
        public string CustomerReference { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTel { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerEmail { get; set; }
    }
}