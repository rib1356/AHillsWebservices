using BatchModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormSizeService
{
    public class FormFilter
    {

       public static bool IsTopiary(Batch b)
        {

            Regex expression = new Regex("Ball|Pyr|Spiral|Hedge|Pyramid|Pon Pon|Bonsai|Cone");
            if (expression.Matches(b.FormSize).Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

       public static Func<Batch, bool> isBulb = delegate (Batch b) {
            Regex expression = new Regex("Bulb|BULB|bulb");
            Regex FSCexpression = new Regex("2BULB");
            return (expression.Matches(b.FormSize).Count > 0 | FSCexpression.Matches(b.FormSizeCode).Count > 0);
        };

       public static Func<Batch, bool> isPot = delegate (Batch b) {
            string potEXpression = @"^[24].+\d[sg]?$|^0.+\d$|^10$|C\d\.$|C$|CON$|\d+C.+$|P11|[CD]\d\d\d?$";
            return (LPotTest(b.FormSizeCode, b.FormSize, potEXpression) | PPotTest(b.FormSizeCode, b.FormSize, potEXpression) | CPotTest(b.FormSizeCode, b.FormSize, potEXpression));
        };

       public static Func<Batch, bool> isBareRoot = delegate (Batch b) {
            return (IsBareRoot(b.FormSizeCode, b.FormSize));
        };

       public static bool IsRootBall(Batch b)
        {
            Regex expressionRB = new Regex("rb|RB|KL|rootball|ROOTBALL");
            Regex FCexpressionRootBall = new Regex("M[A-Z]{3}?$|KLUI$|KLU$|M$");

            MatchCollection IsRootBall = null;

            if (!(String.IsNullOrEmpty(b.FormSizeCode)))
            {
                IsRootBall = FCexpressionRootBall.Matches(b.FormSizeCode);
                if (IsRootBall.Count > 0)
                {
                    return true;
                }
            }
            if (expressionRB.Matches(b.FormSize).Count > 0)
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
                else
                {
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

        private static bool IsBareRoot(string formSizeCode, string formSize)
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
                if (IsBareRoot.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
