/*
 * Author:      Ryan Curran
 * Date:        Jan 2021
 * Description: Class for Object Sales Order Line
 *              
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using Newtonsoft.Json;
using RestSharp;

using BehrensGroup_ClassLibrary.BaseClasses;
using BehrensGroup_ClassLibrary.Object;
using BehrensGroup_ClassLibrary.Functions;

namespace BehrensGroup_ClassLibrary.Transactions
{
    public class SalesOrder_Line : GenericClassTransactionLine
    {
        //ID                    - GenericClassID
        //Number                - GenericClassTransactionNumber
        //CreatedDate           - GenericClassTransactionNumber
        //LineNo                - GenericClassTransactionLine

        public Product Product = new Product();
        public long ProductID { get; set; }

        public string D_ContactName { get; set; }
        public string D_Monogram { get; set; }
        public string D_EmbroideryCode { get; set; }

        public int SaleAgreementType { get; set; }


        public UnitOfMeasure SellingUnits = new UnitOfMeasure();
        public long SellingUnitsID { get; set; }
        public string OriginalOrderLineNo { get; set; }

        public decimal Quantity { get; set; }
        
        public string PricingType { get; set; }
        public decimal NetCost { get; set; }
        public decimal NetPrice { get; set; }
        public decimal DiscountPercentageValue { get; set; }
        public decimal NetPriceLessDiscount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal NetAmountLessDiscount { get; set; }


        public TaxRate TaxRate = new TaxRate();
        public decimal TaxAmount { get; set; }
        public long TaxRateID { get; set; }

        public decimal GrossPrice { get; set; }
        public decimal GrossPriceDiscountAmount { get; set; }

        public decimal Margin { get; set; }

    }
}
