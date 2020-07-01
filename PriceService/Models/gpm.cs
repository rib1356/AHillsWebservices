using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PriceService.Models
{

    public interface IDiscount
    {
        double calculate(double costprice, int gpm);
    }

    public class DiscountStrategy : IDiscount
    {
        public double calculate(double costprice, int gpm)
        {
            double _gpm = (double)gpm/100;
            double result = (costprice / _gpm);
            return result;
        }
    }


    public class calcBL
    {
        IDiscount _strategy;

        public calcBL(IDiscount strategy)
        {
            _strategy = strategy;
        }

        public double GetUnitSalesPrice(double cp)
        {
            int gpm = Gpm.getGpm(cp);
            double result = _strategy.calculate(cp, gpm);
            return result;
        }

    }
}


// newItemPrices.estimatedPrice = (itemList[i].price/((100-obj.gpm)/100)).toFixed(2)
public static class Gpm
    {

    public static int getGpm(double price)
    {

        switch (price)
        {
            case double n when (n <= 0.1):
                return 46;

            case double n when (n > 0.1 && n <= 0.2):
                return 42;

            case double n when (n > 0.2 && n <= 0.35):
                return 35;

            case double n when (n > 0.35 && n <= 0.95):
                return 33;

            case double n when (n > 0.95 && n <= 1.90):
                return 29;

            case double n when (n > 1.90 && n <= 3.00):
                return 24;

            case double n when (n > 3.00 && n <= 6.00):
                return 21;

            case double n when (n > 6.00 && n <= 9.00):
                return 20;

            case double n when (n > 9.00):
                return 19;

            default:
                return -1;


        }

    }


    }
