using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PickListService.DTO
{
    public class PickListDTO
    {
        public int PicklistId { get; set; }
        public int QuoteId { get; set; }
        public DateTime DipatchDate { get; set; }
        public string DeliveryAddress { get; set; }
        public bool DeliveryNeeded { get; set; }
        public bool IsPicked { get; set; }
        public bool IsPacked { get; set; }
        public bool IsDelivered { get; set; }
        public string Comment { get; set; }
        public bool EsimatedDelivery { get; set; }
        public bool Active { get; set; }
    }

    public class SelectedPickListDTO
    {
        public int PicklistId { get; set; }
        public int QuoteId { get; set; }
        public DateTime DipatchDate { get; set; }
        public string DeliveryAddress { get; set; }
        public bool DeliveryNeeded { get; set; }
        public bool IsPicked { get; set; }
        public bool IsPacked { get; set; }
        public bool IsDelivered { get; set; }
        public string Comment { get; set; }
        public bool EsimatedDelivery { get; set; }
        public bool Active { get; set; }
        public IEnumerable<PickListDetailDTO> PickListPlants { get; set; }
    }

    public class PickListDetailDTO
    {
        public int PlantForPicklistId { get; set; }
        public int PicklistId { get; set; }
        public int PlantForQuoteId { get; set; }
        public string PlantName { get; set; }
        public string FormSize { get; set; }
        public int QuantityToPick { get; set; }
        public string SubbedFor { get; set; }
        public bool IsSubbed { get; set; }
        public string DispatchLocation { get; set; }
        public bool Active { get; set; }
    }
}