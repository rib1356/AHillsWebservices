using ImportService.ServiceLayer;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImportService.Excel
{

    public static class CalcData
    {
        /// <summary>
        /// Calculates the proposed sale unit sale price dependant upon form - size
        /// </summary>
        /// <param name="batch"></param>
        public struct Result
        {
            public decimal price;
            public string rule;
        }


        /// <summary>
        ///     Turn 30% into a decimal by dividing 30 by 100, which is 0.3.
        //      Minus 0.3 from 1 to get 0.7.
        //     Divide the price the good cost you by 0.7.
        //    The number that you receive is how much you need to sell the item for to get a 30% profit margin.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="MarginIn"></param>
        /// <returns></returns>

        public static Result CalMarginPrice(ImportModel.Pannebakker batch, decimal MarginIn)
        {
            // Divide the number you wish to increase by 100 to find 1 % of it.
            var Onepc = batch.Price / 100;
            //Multiply 1 % by your chosen percentage.
            var increase = Onepc * MarginIn;
            // Add this number to your original number.
            var newPrice = batch.Price + increase;

              //var margin = MarginIn / 100;
              //// Divide the number you wish to increase by 100 
              //var PcValue = 1 - margin;
              ////   Multiply 1% by your chosen percentage
              //var newPrice = batch.Price / PcValue;

              var result = new Result
            {
                price = 0,
                rule = ""
            };

            // Add this number to your original number
            result.price = newPrice;
            result.rule = "Simple margin of " + MarginIn.ToString() + " applied";
             return result;

        }

        /// <returns>Proposed unit Sale Price</returns>
        public  static Result CalCapPrice(ImportModel.Pannebakker batch)
        {
            //decimal margin = MarginIn / 100;
            // pb buy price
            var y = batch.Price;
            // base sales price
            var x = (batch.Price / 0.55m);

            var result = new Result
            {
                price = 0,
                rule = ""
            };

            ServiceLayer.PriceItemDTO price = PriceService.GetUnitPrice(batch.FormSize, batch.FormSizeCode);
            if (price != null)
            {
                result.rule = price.RuleNumber.ToString();
                var max = Convert.ToDecimal(price.MaxUnitValue * 100);
                var min = Convert.ToDecimal(price.MinUnitValue * 100);
                if (x < min)
                {
                    result.price = min + y;
                    return result;
                }
                if (x > max)
                {
                    result.price = max + y;
                    return result;
                }
            }
            else
            {
                result.price = 0;
                return result;
            }
            result.price = y;
            return result;
        }
    }

    public static class  ExcelData
    {

        public static bool HasData(ExcelWorksheet workSheet, int row)
        {
            if ((workSheet.Cells[row, workSheet.Dimension.Start.Column + 0].Value != null) && (workSheet.Cells[row, workSheet.Dimension.Start.Column + 1].Value != null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // ABEGOUCH
        public static string GetPBSKU(ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + 1].Value != null)
            {
                return (workSheet.Cells[row, workSheet.Dimension.Start.Column + 1].Value).ToString();
            }
            else
            {
                return null;
            }
        }

        // 2C2
        public static string GetPBFSCOde(ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + 2].Value != null)
            {
                return (workSheet.Cells[row, workSheet.Dimension.Start.Column + 2].Value).ToString();
            }
            else
            {
                return null;
            }
        }

        // Abelia 'Edward Goucher'
        public static string GetName(ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + 3].Value != null)
            {
                return (workSheet.Cells[row, workSheet.Dimension.Start.Column + 3].Value).ToString();
            }
            else
            {
                return null;
            }
        }

        // 2 Ltr pot
        public static string GetFSDecription(ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + 4].Value != null)
            {
                return (workSheet.Cells[row, workSheet.Dimension.Start.Column + 4].Value).ToString();
            }
            else
            {
                return null;
            }
        }

        // 3.23
        public static decimal GetPrice(ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + 5].Value != null)
            {
                var data = workSheet.Cells[row, workSheet.Dimension.Start.Column + 5].Value;
                return Convert.ToDecimal(data);
            }
            else
            {
                return 0.0m;
            }
        }

        public static DateTime GetDate(ExcelWorksheet workSheet, int row)
        {
            if (workSheet.Cells[row, workSheet.Dimension.Start.Column + 6].Value != null)
            {
                var data = workSheet.Cells[row, workSheet.Dimension.Start.Column + 6].Value;
                double d = double.Parse(data.ToString());
                DateTime conv = DateTime.FromOADate(d).Date;
                return conv;
            }
            else
            {
                return DateTime.Today;
            }
        }
    }
}