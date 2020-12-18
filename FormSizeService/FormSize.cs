using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormSizeService
{
    public class FormSize
    {

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
    }
}
