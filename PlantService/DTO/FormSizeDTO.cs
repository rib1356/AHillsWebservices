using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantService.DTO
{
    public class FormSizeDTO
    {
        public int id { get; set; }
        public int FormSizeId { get; set; }
        public int GroupId { get; set; }
        public string Age { get; set; }
        public string HeightSpread { get; set; }
        public string Girth { get; set; }
        public double PotSize { get; set; }
        public string RootType { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

    }
}