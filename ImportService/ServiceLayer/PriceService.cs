using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using static ImportService.ServiceLayer.PriceItemDTO;

namespace ImportService.ServiceLayer
{
    public class PriceItemDTO
    {
        //<Description>=>1 and =<3</Description>
        //<MaxUnitValue>1.6</MaxUnitValue>
        //<MinUnitValue>0.4</MinUnitValue>
        //<PlantType>Pot</PlantType>
        //<RuleNumber>2</RuleNumber>
        public enum RootType
        {
            Pot,
            RootBall,
            BareRoot,
            Bulb,
            Topiary
        }

        public int RuleNumber { get; set; }
        public RootType PlantType { get; set; }
        public string Description { get; set; }
        public double MinUnitValue { get; set; }
        public double MaxUnitValue { get; set; }

    }

    public class PriceService
    {

        public static decimal CalCapPrice(ImportModel.Batch batch)
        {
            // pb buy price
            var y = batch.BuyPrice;
            // base sales price
            var x = (batch.BuyPrice / 0.55m);

            PriceItemDTO price = PriceService.GetUnitPrice(batch.FormSize, batch.FormSizeCode);
            if (price != null)
            {
                var max = Convert.ToDecimal(price.MaxUnitValue * 100);
                var min = Convert.ToDecimal(price.MinUnitValue * 100);
                if (x < min)
                {
                    return Convert.ToDecimal(min + y);
                }
                if (x > max)
                {
                    return Convert.ToDecimal(max + y);
                }
            }
            else
            {
                return 0;
            }
            return Convert.ToDecimal(y);
        }

        public static PriceItemDTO GetUnitPrice(string formSize, string formSizeCode)
        {
            var priceItemDTO = new PriceItemDTO();
            var priceItem = PriceService.Get(formSize,formSizeCode);
            if (priceItem != null)
            {
                
                priceItemDTO.Description = "Rule:" + priceItem.RuleNumber  + " : " +  priceItem.Description;
                priceItemDTO.MaxUnitValue = priceItem.MaxUnitValue;
                priceItemDTO.MinUnitValue = priceItem.MinUnitValue;
                priceItemDTO.PlantType = (RootType)priceItem.PlantType;
                priceItemDTO.RuleNumber = priceItem.RuleNumber;
                return priceItemDTO;
            }
            else
            {
                return null;
            }


        }

        private  static PriceRule  Get(string form, string formSizeCode)
        {
            PriceRule priceRule = new PriceRule();
            // this removes all special charaters
            String cleanString = CleanString(form);



            bool IsRB = false;
            bool IsPPot = false;
            bool IsLPot = false;
            bool IsCPot= false;
            bool IsBR = false;
            bool IsBulb = false;
            bool IsTop = false;

            string potEXpression = @"^[24].+\d[sg]?$|^0.+\d$|^10$|C\d\.$|C$|CON$|\d+C.+$|P11|[CD]\d\d\d?$";

            IsPPot = PPotTest(formSizeCode, form, potEXpression);
            IsLPot = LPotTest(formSizeCode, form, potEXpression);
            IsCPot = CPotTest(formSizeCode, form, potEXpression);
            IsTop = TopTest(formSizeCode, form);
            IsBulb = BulbTest(formSizeCode, form);



            // if its  pot then it IS NOT RB or BR
            if (IsPPot | IsCPot | IsLPot)
            {
                IsRB = false;
                IsBR = false;
            }
            else
            {
                IsRB  = RootBallTest(formSizeCode, form);
                if (!IsRB)
                {
                    IsBR = BareRootTest(formSizeCode, form);
                }
            }


            switch (true)
            {
                case bool _ when IsBulb:

                    return PriceRule.getBulbRule();


                case bool _ when IsTop:

                    return PriceRule.getTopiaryRule();


                case bool _ when IsRB:
                    // split string with space
                    string[] words = form.Split(' ');
                    // find string with numbers
                    string size = "";
                    foreach (var word in words)
                    {
                        if (word.Any(char.IsDigit))
                        {
                            size = word;
                            break;
                        }
                    }
                    // split that string with -
                    string[] sizes = size.Split('-');
                    // get low value get high value
                    var resultRB = readRBForm("RB", sizes);
                    return resultRB;


                case bool _ when IsCPot:

                    string cSize = "";
                   
                    // split string with space
                    string[] cWords = form.Split(' ');
                    
                    Regex expressionC = new Regex("[C][0-9]|CONT");
                    MatchCollection hasAc = expressionC.Matches(form);
                    foreach (var word in cWords)
                    {
                        if (expressionC.Matches(word).Count > 0)
                        {
                            cSize = word;
                            break;
                        }
                    }

                    Regex specialC = new Regex("2C1.4");
                    if (specialC.Matches(formSizeCode).Count > 0)
                    {
                        cSize = "1.5";
                    }

                    var resultC = PriceRule.getPotRule("C", cSize);
                    return resultC;


                case bool _ when IsPPot:
                    // return readForm("P", cleanString);
                    string[] pWords = form.Split(' ');
                    string pSize = "";
                    Regex expressionP = new Regex("[P][0-9]");
                    MatchCollection hasAp = expressionP.Matches(form);
                    foreach (var word in pWords)
                    {
                        if (expressionP.Matches(word).Count > 0)
                        {
                            pSize = word;
                            break;
                        }
                    }
                    //var resultP = getPotRule("P", pSize);
                    var resultP = PriceRule.getPotRule("P", pSize);
                    return resultP;

                case bool _ when IsLPot:
                    string[] lWords = form.Split(' ');
                    string lSize = "";
                    foreach (var word in lWords)
                    {
                        string thisWord = CleanString(word);
                        if (Char.IsDigit(thisWord[0]))
                        {
                            lSize = word;
                            break;
                        }
                    }
                    var resultL = PriceRule.getPotRule("L", lSize);
                    return resultL;




                case bool _ when IsBR:
                    string BRsize;
                    return ProcessBareRoot(form, out BRsize);

                default:
                    string BRdsize;
                    return ProcessBareRoot(form, out BRdsize);
            }


        }

        private static PriceRule ProcessBareRoot(string form, out string BRsize)
        {
            string[] BRwords = form.Split(' ');
            // find string with numbers
            BRsize = "";
            foreach (var word in BRwords)
            {
                if (word.Any(char.IsDigit) && !(word.Contains("/")) )
                {
                    BRsize = word;
                    break;
                }
            }
            // split that string with -
            string[] BRsizes = BRsize.Split('-');
            // get low value get high value
            if (BRsizes.Length == 2)
            {
                var resultBR = readBRForm("BR", BRsizes);
                return resultBR;
            }
            else
            {
                return readBRForm("BR", BRsizes);
            }
        }

        private static bool TopTest(string formSizeCode, string form)
        {

            Regex expression = new Regex("Ball|Pyr|Spiral|Hedge|Pyramid|Pon Pon|Bonsai|Cone");
            if (expression.Matches(form).Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool BulbTest(string formSizeCode, string form)
        {

            Regex expression = new Regex("Bulb|BULB|bulb");
            Regex FSCexpression = new Regex("2BULB");
            // 2BULB
            if (expression.Matches(form).Count > 0 | FSCexpression.Matches(formSizeCode).Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool RootBallTest(string formSizeCode, string form)
        {
            Regex expressionRB = new Regex("rb|RB|KL|rootball|ROOTBALL");

            Regex FCexpressionRootBall = new Regex("M[A-Z]{3}?$|KLUI$|KLU$|M$");
            MatchCollection IsRootBall = null;

            if (!(String.IsNullOrEmpty(formSizeCode)))
            {
                IsRootBall = FCexpressionRootBall.Matches(formSizeCode);
                if (IsRootBall.Count > 0)
                {
                    return true;
                }
            }

            if (expressionRB.Matches(form).Count > 0)
            {
                return true;
            }

 

            return false;
        }

        private static bool LPotTest(string formSizeCode, string form, string expression)
        {
            MatchCollection IsPot = null;
            Regex expressionL = new Regex("[0-9] L|CONT|cont");
            MatchCollection hasAL = expressionL.Matches(form);
            Regex expressionPot = new Regex(expression);
            if (!(String.IsNullOrEmpty(formSizeCode)))
            {
                IsPot = expressionPot.Matches(formSizeCode);
                if (hasAL.Count > 0)
                {
                    return true;
                }
            }
            else
            {
                if (hasAL.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool CPotTest(string formSizeCode, string form, string expression)
        {
            MatchCollection IsPot = null;
            Regex expressionC = new Regex("[C][0-9]");
            MatchCollection hasAc = expressionC.Matches(form);
            Regex expressionPot = new Regex(expression);
            if (!(String.IsNullOrEmpty(formSizeCode)))
            {
                Regex specialC = new Regex("2C1.4");
                if (specialC.Matches(formSizeCode).Count > 0)
                {
                    return true;
                }
                else {
                        IsPot = expressionPot.Matches(formSizeCode);
                        if (IsPot.Count > 0 && hasAc.Count > 0)
                        {
                            return true;
                        }
                }
            }
            else
            {
                if (hasAc.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool PPotTest(string formSizeCode, string form, string expression)
        {
            MatchCollection IsPot = null;
            Regex expressionPot = new Regex(expression);
            Regex expressionP = new Regex("[P][0-9]");
            MatchCollection hasAp = expressionP.Matches(form);
            if (!(String.IsNullOrEmpty(formSizeCode)))
            {
                IsPot = expressionPot.Matches(formSizeCode);
                if (hasAp.Count > 0)
                {
                    return true;
                }
            }
            else
            {
                if (hasAp.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool BareRootTest(string formSizeCode, string formSize)
        {
            var testString = formSize.ToLower();
            Regex FSCexpressionBareRoot = new Regex("^2.+[ZT]$");
            Regex myFSCexpression = new Regex("10Z");
            Regex FSexpressionBareRoot = new Regex("bare root");
            MatchCollection hasBR = FSexpressionBareRoot.Matches(testString);
           // MatchCollection hasmyBR = myFSCexpression.Matches(formSizeCode);
            MatchCollection IsBareRoot = null;
            MatchCollection IsMYBareRoot = null;
            if (!(String.IsNullOrEmpty(formSizeCode)))
            {
                IsBareRoot = FSCexpressionBareRoot.Matches(formSizeCode);
                IsMYBareRoot = myFSCexpression.Matches(formSizeCode);
                if (IsBareRoot.Count > 0 | IsMYBareRoot.Count > 0)
                {
                    return true;
                }
            }


            if (hasBR.Count > 0)
            {
                return true;
            }

            if (IsBareRoot != null)
            {
              if(IsBareRoot.Count > 0)
                {
                    return true;
                }
            }

            return false;

   
        }

        private static PriceRule readRBForm(string Root, string[] sizes)
        {
            string lowNum = "0";
            string highNum = "0";
            try {
                if(!String.IsNullOrEmpty(sizes[0].ToString()))
                {
                    lowNum = CleanNumbers(CleanString(sizes[0].ToString()));
                }
                if (sizes.Length > 1 )
                {
                    if (!String.IsNullOrEmpty(sizes[1].ToString()))
                    {
                        highNum = CleanNumbers(CleanString(sizes[1].ToString()));
                    }
                }
                
                return PriceRule.getRBRule("RB", lowNum, highNum);
            }
            catch{
                return PriceRule.getRBRule("RB", lowNum, highNum);
            }


        }

        private static PriceRule readBRForm(string Root, string[] sizes)
        {
            string lowNum = "0";
            string highNum = "0";
            try
            {
                if (!String.IsNullOrEmpty(sizes[0].ToString()))
                {
                    lowNum = CleanNumbers(CleanString(sizes[0].ToString()));
                }
                if (sizes.Length > 1 & !String.IsNullOrEmpty(sizes[1].ToString()))
                {
                    highNum = CleanNumbers(CleanString(sizes[1].ToString()));
                }

                return PriceRule.getBRRule("BR", lowNum, highNum);
            }
            catch
            {
                return PriceRule.getBRRule("BR", lowNum, highNum);
            }


        }

        private static string readForm(string root, string cleanString)
        {
            var numPart = CleanNumbers(CleanString(cleanString));
            if (Decimal.TryParse(numPart, out decimal size))
            {
                return "got a " + root + " with " + size.ToString();
            }
            else
            {
                return "String could not be parsed.";
            }
        }

        public static string CleanString(string text)
        {

            text = text.Replace("SOLITAIRE", "");
            text = text.Replace("SOLITAIR", "");
            StringBuilder sb = new StringBuilder(text.Length);

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == '.')
                    sb.Append(text[i]);
            }

            return sb.ToString();
        }

        public static string CleanNumbers(string text)
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


    }
}