using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantService.DTO
{
    public class EditPlantDTO
    {
        public int PlantId { get; set; }
        public IEnumerable<GroupDTO> GroupDetails { get; set; }
    }


    public class NewPlantDTO
    {
        public string Name { get; set; }
        public string Sku { get; set; }
        public IEnumerable<GroupDTO> GroupDetails { get; set; }
    }

    public class GroupDTO
    {
        public int GroupId { get; set; }
        public string GroupDescription { get; set; }

    }
}