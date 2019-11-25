using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteService.DTO
{
    public class PickListDTO
    {
        public int PicklistId { get; set; }
        public int QuoteId { get; set; }
        public string DispatchDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string CustomerName { get; set; }
        public bool DeliveryNeeded { get; set; }
        public bool IsPicked { get; set; }
        public bool IsAllocated { get; set; }
        public bool IsDelivered { get; set; }
        public string Comment { get; set; }
        public bool EstimatedDelivery { get; set; }
        public int PickListItemQty { get; set; }
        public int TotalAmountPicked { get; set; }
        public bool Active { get; set; }
    }

    public class SelectedPickListDTO
    {
        public int PicklistId { get; set; }
        public int QuoteId { get; set; }
        public string DispatchDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string CustomerRef { get; set; }
        public string CustomerTel { get; set; }
        public bool DeliveryNeeded { get; set; }
        public bool IsPicked { get; set; }
        public bool IsAllocated { get; set; }
        public bool IsDelivered { get; set; }
        public string Comment { get; set; }
        public bool EstimatedDelivery { get; set; }
        public int PickListItemQty { get; set; }
        public int TotalAmountPicked { get; set; }
        public bool Active { get; set; }
        public IEnumerable<PickListDetailDTO> PickListPlants { get; set; }
    }

    public class PickListDetailDTO
    {
        public int PlantForPicklistId { get; set; }
        public int PicklistId { get; set; }
        public int PlantForQuoteId { get; set; }
        public int BatchId { get; set; }
        public string BatchLocation { get; set; }
        public string PlantName { get; set; }
        public string FormSize { get; set; }
        public int QuantityToPick { get; set; }
        public int QuantityPicked { get; set; }
        public string SubbedFor { get; set; }
        public bool IsSubbed { get; set; }
        public string DispatchLocation { get; set; }
        public bool Active { get; set; }
    }

    public class QuotePlantsNeededDTO
    {

    }
}