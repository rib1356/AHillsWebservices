using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace quoteService.DTO
{
    public class QuoteDTO
    {
        public int QuoteId { get; set; }
        public string CustomerRef { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string Date { get; set; }
        public string ExpiryDate { get; set; }
        public string SiteRef { get; set; }
        public int TotalPrice { get; set; }
        public bool SalesOrder { get; set; }
        public bool Retail { get; set; }
        public bool Active { get; set; }
    }

    //public class CustomerDTO
    //{
    //    public string CustomerName { get; set; }
    //    public string CustomerAddress { get; set; }

    //}
}