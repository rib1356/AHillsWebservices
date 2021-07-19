using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Purchasing1.Models
{
    public class PurchaseOrderVM
    {
        public int CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public int OrderNumber { get; set; }
        public List<PurchaseOrderItemVM> purchaseOrderItems { get;set; }
    }

    public class PurchaseOrderItemVM
    {
        public string PlantName { get; set; }
        public string FormSize { get; set; }
        public int QuantityRequied { get; set; }      
        public decimal BatchUnitPrice { get; set; }
    }
}