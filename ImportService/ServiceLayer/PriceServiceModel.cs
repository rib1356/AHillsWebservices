using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ImportService.ServiceLayer
{
    public class PriceRule
    {
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

        public static PriceRule getRBRule(string root, string min, string max)
        {

            try {
                var minNumPart = CleanNumbers(CleanString(min));
                var maxNumPart = CleanNumbers(CleanString(max));
                decimal minM = Decimal.Parse(minNumPart);
                decimal maxM = Decimal.Parse(maxNumPart);

                if (maxM - minM == 2)
                {
                    if (minM <= 10.0m)
                    {
                        return PriceRules.Rules.Single(r => r.RuleNumber == 7);
                    }
                    if (minM > 10.0m && maxM <= 18.0m)
                    {
                        return PriceRules.Rules.Single(r => r.RuleNumber == 8);
                    }
                    // min>=18 && max<=30
                    if (minM >= 18 && maxM <= 30)
                    {
                        return PriceRules.Rules.Single(r => r.RuleNumber == 9);
                    }
                }
                else if (maxM - minM > 4.0m) // need a 100 to 125 Rule
                {
                    if (maxM <= 60) // changed on 11/9
                    {
                        return PriceRules.Rules.Single(r => r.RuleNumber == 10);
                    }
                    if (minM >= 60 && maxM <= 100) // new rule 11/9
                    {
                        return PriceRules.Rules.Single(r => r.RuleNumber == 20);
                    }
                    if (minM >= 100 && maxM <= 150) // updated 11/9
                    {
                        return PriceRules.Rules.Single(r => r.RuleNumber == 11);
                    }
                    // min>=18 && max<=30
                    if (minM >= 150 && maxM <= 250)
                    {
                        return PriceRules.Rules.Single(r => r.RuleNumber == 12);
                    }
                    if (maxM > 250)
                    {
                        return PriceRules.Rules.Single(r => r.RuleNumber == 13);
                    }
                }
            }
            catch(Exception ex)
            {
                return null;
            }
            



            return null;
        }

        public static PriceRule getPotRule(string root, string cleanString)
        {
            try {
                var numPart = CleanNumbers(CleanString(cleanString));
                if (Decimal.TryParse(numPart, out decimal size))
                {
                    if (size > 30.0m)
                    {
                        // then 6
                        return PriceRules.Rules.Single(r => r.RuleNumber == 6);
                    }
                    if ((size < 1.0m) || (size == 9 && root == "P"))
                    {
                        // then 1
                        return PriceRules.Rules.Single(r => r.RuleNumber == 1);
                    }
                    if (size >= 1.0m && size <= 3.0m || (root == "P"))
                    {
                        // then 2
                        return PriceRules.Rules.Single(r => r.RuleNumber == 2);
                    }
                    if (size > 3.0m && size <= 7.5m)
                    {
                        // then 3
                        return PriceRules.Rules.Single(r => r.RuleNumber == 3);
                    }
                    if (size > 7.5m && size <= 15.0m)
                    {
                        // then 4
                        return PriceRules.Rules.Single(r => r.RuleNumber == 4);
                    }
                    if (size > 15.0m && size <= 30.0m)
                    {
                        // then 5
                        return PriceRules.Rules.Single(r => r.RuleNumber == 5);
                    }

                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
            
            return null;
        }

        private static string CleanString(string text)
        {
            StringBuilder sb = new StringBuilder(text.Length);

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == '.')
                    sb.Append(text[i]);
            }

            return sb.ToString();
        }

        private static string CleanNumbers(string text)
        {
            StringBuilder sb = new StringBuilder(text.Length);

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c >= '0' && c <= '9' || c == '.')
                    sb.Append(text[i]);
            }

            return sb.ToString();
        }

        public static PriceRule getBRRule(string Root, string min, string max)
        {
            try
            {
                var minNumPart = CleanNumbers(CleanString(min));
                var maxNumPart = CleanNumbers(CleanString(max));
                decimal minM = Decimal.Parse(minNumPart);
                decimal maxM = Decimal.Parse(maxNumPart);

                if (maxM - minM == 2)
                {
                    // max<12
                    if (maxM <= 12.0m)
                    {
                        return PriceRules.Rules.Single(r => r.RuleNumber == 14);
                    }
                    if (minM > 12.0m)
                    {
                        return PriceRules.Rules.Single(r => r.RuleNumber == 15);
                    }
                }
                else if (maxM - minM > 4.0m)
                {
                    if (maxM <= 60)
                    {
                        return PriceRules.Rules.Single(r => r.RuleNumber == 16);
                    }
                    if (minM > 60 && maxM <= 150)
                    {
                        return PriceRules.Rules.Single(r => r.RuleNumber == 17);
                    }
                    // min>150 && max<=250
                    if (minM >= 150 && maxM <= 250)
                    {
                        return PriceRules.Rules.Single(r => r.RuleNumber == 18);
                    }
                    if (minM >= 250)
                    {
                        return PriceRules.Rules.Single(r => r.RuleNumber == 19);
                    }
                }
            }
            catch(Exception ex)
            {
                return null;
            }



            return null;
        }
    }

    public static class PriceRules
    {
        public static List<PriceRule> Rules = new List<PriceRule>() {
                    new PriceRule() {  RuleNumber = 1, PlantType = PriceRule.RootType.Pot, Description = "<1 or P9", MinUnitValue=0.2, MaxUnitValue= 0.9} ,
                    new PriceRule() {  RuleNumber = 2, PlantType = PriceRule.RootType.Pot, Description = "=>1 and =<3", MinUnitValue=0.4, MaxUnitValue= 1.6}  ,
                    new PriceRule() {  RuleNumber = 3, PlantType = PriceRule.RootType.Pot, Description = ">3 and =<7.5", MinUnitValue=2.5, MaxUnitValue= 4.0}  ,
                    new PriceRule() {  RuleNumber = 4, PlantType = PriceRule.RootType.Pot, Description = ">7.5 =<15 ( not p9 )", MinUnitValue=4.0, MaxUnitValue= 9.0}  ,
                    new PriceRule() {  RuleNumber = 5, PlantType = PriceRule.RootType.Pot, Description = ">15 =<30", MinUnitValue=9.0, MaxUnitValue=15.0}  ,
                    new PriceRule() {  RuleNumber = 6, PlantType = PriceRule.RootType.Pot, Description = ">30", MinUnitValue=15.0, MaxUnitValue= 40.0} ,
                    new PriceRule() {  RuleNumber = 7, PlantType = PriceRule.RootType.RootBall, Description = "range=2 and min<=10", MinUnitValue=9.0, MaxUnitValue= 15.0} ,
                    new PriceRule() {  RuleNumber = 8, PlantType = PriceRule.RootType.RootBall, Description = "range=2 and min>10 and max<=18", MinUnitValue=10.0, MaxUnitValue=40.0}  ,
                    new PriceRule() {  RuleNumber = 9, PlantType = PriceRule.RootType.RootBall, Description = "range=2 && min>=18 && max<=30", MinUnitValue=30.0, MaxUnitValue=60.0}  ,
                    new PriceRule() {  RuleNumber = 10, PlantType = PriceRule.RootType.RootBall, Description = "range>4 && max<=60", MinUnitValue=3.0, MaxUnitValue= 9.0}  ,
                    new PriceRule() {  RuleNumber = 20, PlantType = PriceRule.RootType.RootBall, Description = "range>4 && min>60 max<=100", MinUnitValue=9.0, MaxUnitValue= 12.0},
                    new PriceRule() {  RuleNumber = 11, PlantType = PriceRule.RootType.RootBall, Description = "range>4 && min>100 && max<=150", MinUnitValue=12.0, MaxUnitValue=30.0}  ,
                    new PriceRule() {  RuleNumber = 12, PlantType = PriceRule.RootType.RootBall, Description = "range>4 && min>150 && max<=250", MinUnitValue=20.0, MaxUnitValue= 40.0},
                    new PriceRule() {  RuleNumber = 13, PlantType = PriceRule.RootType.RootBall, Description = "range>4 && min>250", MinUnitValue=35.0, MaxUnitValue= 60.0},
                    new PriceRule() {  RuleNumber = 14, PlantType = PriceRule.RootType.BareRoot, Description = "range=6 && max<12", MinUnitValue=9.0, MaxUnitValue=15.0}  ,
                    new PriceRule() {  RuleNumber = 15, PlantType = PriceRule.RootType.BareRoot, Description = "range=12 && min>18", MinUnitValue=10.0, MaxUnitValue=35.0}  ,
                    new PriceRule() {  RuleNumber = 16, PlantType = PriceRule.RootType.BareRoot, Description = "range> 4 && max<=60", MinUnitValue=0.2, MaxUnitValue= 0.9}  ,
                    new PriceRule() {  RuleNumber = 17, PlantType = PriceRule.RootType.BareRoot, Description = "range> 4 && min>60 && max<=150", MinUnitValue=0.6, MaxUnitValue=0.2}  ,
                    new PriceRule() {  RuleNumber = 18, PlantType = PriceRule.RootType.BareRoot, Description = "range>4 && min>150 && max<=250", MinUnitValue=2.0, MaxUnitValue= 8.0},
                    new PriceRule() {  RuleNumber = 19, PlantType = PriceRule.RootType.BareRoot, Description = "range>4 && min>250", MinUnitValue=8.0, MaxUnitValue= 15.0},
                };
    }
}