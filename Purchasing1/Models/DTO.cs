using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Purchasing1.Models
{

    public class purchaseOrderDTO
    {
        public int orderid { get; set; }
        public string custormerRef { get; set; }
        public DateTime QuoteDate { get; set; }
    }

    public class PurchaseListDTO
    {

        public List<PurchaseItemDTO> Items { get; set; }
        public int Count { get; set; }
    }

    public class PurchaseItemDTO
    {
        public int batchid { get; set; }
        public string sku { get; set; }
        public string plantName { get; set; }
        public string formSize { get; set; }

    }
}