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
            BareRoot
        }

        public int RuleNumber { get; set; }
        public RootType PlantType { get; set; }
        public string Description { get; set; }
        public double MinUnitValue { get; set; }
        public double MaxUnitValue { get; set; }

    }

    public class PriceService
    {
        /// <summary>
        /// WE need to modify the System URi to use the deployed services
        /// </summary>
        /// <returns></returns>
        //private static HttpClient ApiClient()
        //{
        //    HttpClient client = new HttpClient();
        //    // http://localhost:52009/
        //    //client.BaseAddress = new System.Uri("http://localhost:61628/");
        //    client.BaseAddress = new System.Uri("https://priceservice.azurewebsites.net/");
        //    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        //    return client;
        //}

        //public static PriceItemDTO GetPUnitPrice(string formSize)
        //{
        //    HttpClient client = ApiClient();
        //    var request = "api/Values?form=" + formSize;
        //    HttpResponseMessage response = client.GetAsync(request).Result;
        //    if (response.IsSuccessStatusCode)
        //    {
        //        return response.Content.ReadAsAsync<PriceItemDTO>().Result;
        //    }
        //    else
        //    {
        //        Debug.WriteLine("Index received a bad response from the web service.");
        //        return null;
        //    }


        //}


        public static PriceItemDTO GetUnitPrice(string formSize, string formSizeCode)
        {
            var priceItemDTO = new PriceItemDTO();
            var priceItem = PriceService.Get(formSize,formSizeCode);
            if (priceItem != null)
            {
                
                priceItemDTO.Description = priceItem.Description;
                priceItemDTO.MaxUnitValue = priceItem.MaxUnitValue;
                priceItemDTO.MinUnitValue = priceItem.MinUnitValue;
                priceItemDTO.PlantType = (RootType)priceItem.PlantType;
                priceItem.RuleNumber = priceItem.RuleNumber;
                return priceItemDTO;
            }
            else
            {
                return null;
            }


        }

        private  static PriceRule  Get(string form, string formSizeCode)
        {

            if (form == "125-150 C20 1 CANE")
            {
                var fred = true;
            }
            PriceRule priceRule = new PriceRule();
            // this removes all special charaters
            String cleanString = CleanString(form);

            // Pot
            // ^[24].+\d[sg]?$|^0.+\d$|^10$|C\d\.$|C$|CON$|\d+C.+$|P11|[CD]\d\d\d?$
            string potEXpression = @"^[24].+\d[sg]?$|^0.+\d$|^10$|C\d\.$|C$|CON$|\d+C.+$|P11|[CD]\d\d\d?$";
            Regex expressionPot = new Regex(potEXpression);
            Regex expressionBareRoot = new Regex("^2.+[ZT]$");
            Regex expressionRootBall = new Regex("M[A-Z]{3}?$|KLUI$|KLU$|M$");



            Regex expressionC = new Regex("[C][0-9]");
            Regex expressionP = new Regex("[P][0-9]");
            Regex expressionL = new Regex("[0-9] L");
            Regex expressionCon = new Regex("CONT");
            Regex expressionRB = new Regex("RB|KL|rootball|ROOTBALL");
            Regex expressionRBFSC = new Regex("M");
            Regex expressionSTD = new Regex("STD|bare root|BARE ROOT|FEATHERED");
            //Regex expressionL = new Regex("[C][0-9]");
            MatchCollection IsPot = null;
            MatchCollection IsBareRoot = null;
            MatchCollection IsRootBall = null;
            MatchCollection hasAcFC = null;
            MatchCollection hasApFC = null;
            MatchCollection hasARBFC = null;

            if (!( String.IsNullOrEmpty(formSizeCode)))
            {
                IsPot = expressionPot.Matches(formSizeCode);
                IsBareRoot = expressionBareRoot.Matches(formSizeCode);
                IsRootBall = expressionRootBall.Matches(formSizeCode);
                hasAcFC = expressionC.Matches(formSizeCode);
                hasApFC = expressionP.Matches(formSizeCode);
                hasARBFC = expressionRB.Matches(formSizeCode);
            }
            var hasAc = expressionC.Matches(form); 
            var hasAp = expressionP.Matches(form);            
            var hasAL = expressionL.Matches(form);
            var hasSTD = expressionSTD.Matches(form);
            var hasARB = expressionRB.Matches(form);           
            var hasCont = expressionCon.Matches(form);



            bool isBR = false;
            if ((hasARB.Count == 0) && (hasAc.Count == 0) && (hasAp.Count == 0) && (hasAL.Count == 0) && (hasCont.Count == 0))
            {
                isBR = true;
            }

            /// check rootball status
            bool rootBall = false;
            if (IsRootBall == null)
            {
                if (hasARB.Count > 0)
                {
                    rootBall = true;
                }
            }
            else
            {
                if (IsRootBall.Count > 0)
                {
                    rootBall = true;
                }
            }


            // chack pot C
            bool IsCPOT = false;
            if(IsPot == null && ( hasAc.Count > 0 | hasCont.Count > 0 ))
            {              
                    IsCPOT = true;
            }

            //IsPot.Count > 0 && hasAp.Count > 0:
            bool IsPPOT = false;
            if (IsPot == null && hasAp.Count > 0)
            {
                IsPPOT = true;
            }

            // IsPot.Count > 0 && hasAL.Count > 0
            bool IsLPOT = false;
            if (IsPot == null && hasAL.Count > 0)
            {
                IsLPOT = true;
            }

            // IsBareRoot.Count > 0  | isBR
            bool BareRootIam = false;
            if (IsBareRoot == null )
            {
                if(isBR)
                {
                    BareRootIam = true;
                } 
            }
            else
            {
                if (IsBareRoot.Count > 0)
                {
                    BareRootIam = true;
                }
            }


            switch (true)
            {
                case bool _ when rootBall:
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


                case bool _ when IsCPOT:
                    // split string with space
                    string[] cWords = form.Split(' ');
                    string cSize = "";
                    foreach (var word in cWords)
                    {
                        if (expressionC.Matches(word).Count > 0)
                        {
                            cSize = word;
                            break;
                        }
                    }
                    //return readForm("C", cSize);
                    //var resultC = getPotRule("C",cSize);
                    var resultC = PriceRule.getPotRule("C", cSize);
                    return resultC;


                case bool _ when IsPPOT:
                    // return readForm("P", cleanString);
                    string[] pWords = form.Split(' ');
                    string pSize = "";
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

                case bool _ when IsLPOT:
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
                    //var resultL = getPotRule("L", lSize);
                    var resultL = PriceRule.getPotRule("L", lSize);
                    return resultL;




                case bool _ when BareRootIam:
                    string[] BRwords = form.Split(' ');
                    // find string with numbers
                    string BRsize = "";
                    foreach (var word in BRwords)
                    {
                        if (word.Any(char.IsDigit))
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
                        return null;
                    }

                default:
                    return null;
            }


        }

        private static PriceRule readRBForm(string Root, string[] sizes)
        {
            try {
                var lowNum = CleanNumbers(CleanString(sizes[0].ToString()));
                var highNum = CleanNumbers(CleanString(sizes[1].ToString()));
                return PriceRule.getRBRule("RB", lowNum, highNum);
            }
            catch{
                return null;
            }


        }

        private static PriceRule readBRForm(string Root, string[] sizes)
        {
            try {
                var lowNum = CleanNumbers(CleanString(sizes[0].ToString()));
                var highNum = CleanNumbers(CleanString(sizes[1].ToString()));
                return PriceRule.getBRRule("BR", lowNum, highNum);
            }
            catch {
                return null;
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