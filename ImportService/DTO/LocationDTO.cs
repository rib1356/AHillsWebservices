using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImportService.DTO
{
    public class LocationDTO
    {
        public int LocationId { get; set; }
        public string MainLocation { get; set; }
        public string SubLocation { get; set; }
        public Nullable<int> WalkOrder { get; set; }
        public Nullable<bool> Active { get; set; }
    }
}