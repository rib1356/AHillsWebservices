using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace PriceService.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }



        

       //Regex expression = new Regex(@"%download%#(?<Identifier>[0-9]*)");
       //  var results = expression.Matches(input);


        // var matchVar = string.match(/^C[0-9]+$/);
        public Models.PriceRule Get(string form)
        {
            Models.PriceRule priceRule = new Models.PriceRule();
            // this removes all special charaters
            String cleanString = CleanString(form);

            Regex expressionC = new Regex("[C][0-9]");
            Regex expressionP = new Regex("[P][0-9]");
            Regex expressionL = new Regex("[0-9] L");
            Regex expressionRB = new Regex("RB|KL|rootball|ROOTBALL");
            Regex expressionSTD = new Regex("STD|bare root|BARE ROOT|FEATHERED" );
            //Regex expressionL = new Regex("[C][0-9]");

            var hasAc = expressionC.Matches(form);
            var hasAp = expressionP.Matches(form);
            var hasAL = expressionL.Matches(form);
            var hasSTD = expressionSTD.Matches(form);
            var hasARB = expressionRB.Matches(form);

    

            bool isBR = false;
            if ((hasARB.Count == 0) && (hasAc.Count == 0) && (hasAp.Count == 0) && (hasAL.Count == 0))
            {
                isBR = true;
            }


            switch (true)
            {
                case bool _ when hasARB.Count > 0:
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


                case bool _ when (hasAc.Count > 0):
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
                    var resultC = priceRule.getPotRule("C", cSize);
                    return resultC;


                case bool _ when hasAp.Count > 0:
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
                    var resultP = priceRule.getPotRule("P", pSize);
                    return resultP;

                case bool _ when hasAL.Count > 0 :
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
                    var resultL = priceRule.getPotRule("L", lSize);
                    return resultL;




                case bool _ when (hasSTD.Count > 0 | isBR):
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
                    var resultBR = readBRForm("BR", BRsizes);
                    return resultBR;

                default:
                   return null;
            }


        }

        private Models.PriceRule readRBForm(string Root, string[] sizes)
        {           
                var lowNum = CleanNumbers(CleanString(sizes[0].ToString()));
                var highNum = CleanNumbers(CleanString(sizes[1].ToString()));
                return Models.PriceRule.getRBRule("RB", lowNum, highNum);
        }

        private Models.PriceRule readBRForm(string Root, string[] sizes)
        {
            var lowNum = CleanNumbers(CleanString(sizes[0].ToString()));
            var highNum = CleanNumbers(CleanString(sizes[1].ToString()));
            return Models.PriceRule.getBRRule("BR", lowNum, highNum);
        }

        private static string readForm(string root, string cleanString)
        {
            var numPart = CleanNumbers(CleanString(cleanString));
            if (Decimal.TryParse(numPart, out decimal size))
            {
                return "got a "  + root + " with " + size.ToString();
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
                if ( c >= '0' && c <= '9' || c == '.')
                    sb.Append(text[i]);
            }

            return sb.ToString();
        }



        // GET api/values/5
        public string Get(double cost)
        {
            Models.calcBL discounter = new Models.calcBL(new Models.DiscountStrategy());
            return discounter.GetUnitSalesPrice(cost).ToString();

        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
