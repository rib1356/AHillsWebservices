﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatchService.Models
{
    public class BatchPriceDTO
    {
        public int BatchId { get; set; }
        public int Price { get; set; }
    }

    public class BatchLocationDTO
    {
        public int BatchId { get; set; }
        public string Location { get; set; }
        public int Quantity { get; set; }
        public int GrowingQuantity { get; set; }
    }


    public class BatchListDTO
    {
        
        public List<BatchItemDTO> Items {get;set;}
        public int Count { get; set; }
    }

    public class BatchItemDTO
    {
        public int Id { get; set; } 
        public string Sku { get; set; }
        public string Name { get; set; }
        public string FormSize { get; set; }
        public string FormSizeCode { get; set; }
        public string Location { get; set; }
        public int Quantity { get; set; }
        public int GrowingQuantity { get; set; }
        public Nullable<int> WholesalePrice { get; set; }
        public Nullable<int> PurchasePrice { get; set; }
        public string Comment { get; set; }
    }

    public class BatchFormDTO
    {
        
        public string Sku { get; set; }
        public string Name { get; set; }
        public string FormSize { get; set; }
        public string FormSizeCode { get; set; }
        public decimal WholesalePrice { get; set; }

    }
}