using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace quoteService.DTO
{
    public class SelectedQuoteDTO
    {
        public int QuoteId { get; set; }
        public string CustomerRef { get; set; }
        public int TotalPrice { get; set; }
        public string SiteRef { get; set; }
        public IEnumerable<QuoteDetailDTO> QuoteDetails { get; set; }
    }

    public class NewQuoteDTO
    {
        public string CustomerRef { get; set; }
        public int TotalPrice { get; set; }
        public string Date { get; set; }
        public string ExpiryDate { get; set; }
        public string SiteRef { get; set; }
        public bool SalesOrder { get; set; }
        public bool Retail { get; set; }
        public bool Active { get; set; }
        public IEnumerable<QuoteDetailDTO> QuoteDetails { get; set; }
    }

    public class QuoteDetailDTO
    {
        public int PlantForQuoteId { get; set; }
        public string PlantName { get; set; }
        public string FormSize { get; set; }
        public string Comment { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public bool Active { get; set; }
    }
}