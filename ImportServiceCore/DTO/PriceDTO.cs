using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportServiceCore.DTO
{
  public class PriceItemDTO
        {
            //        <Description>=>1 and =<3</Description>
            //<MaxUnitValue>1.6</MaxUnitValue>
            //<MinUnitValue>0.4</MinUnitValue>
            //<PlantType>Pot</PlantType>
            //<RuleNumber>2</RuleNumber>
            public enum RootType
            {
                Pot,
                RootBall,
                BareRoot
            }

            public int RuleNumber { get; set; }
            public RootType PlantType { get; set; }
            public string Description { get; set; }
            public double MinUnitValue { get; set; }
            public double MaxUnitValue { get; set; }

        }

}
