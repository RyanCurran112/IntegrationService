/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Model for Transaction Line Sales Order Line
 *              
 */

using System;
using System.Collections.Generic;
using System.Text;

using Behrens_iQ_ClassLibrary.BaseClasses;
using Behrens_iQ_ClassLibrary.Models.Object;

namespace Behrens_iQ_ClassLibrary.Models.Transaction
{
    public class SalesOrder_Line : GenericClassTransactionLine
    {
        //ID                    - GenericClassID
        //CreatedOn             - GenericClassID
        //UpdatedOn             - GenericClassID
        //Number                - GenericClassTransactionNumber
        //CreatedDate           - GenericClassTransactionNumber
        //LineNo                - GenericClassTransactionLine

        public Product Product = new Product();
        public long ProductID { get; set; }

        public string D_ContactName { get; set; }
        public string D_Monogram { get; set; }
        public string D_EmbroideryCode { get; set; }

        public decimal Quantity { get; set; }

        public UnitOfMeasure SellingUnits { get; set; }
        public long SellingUnitsID { get; set; }

        public string PricingType { get; set; }
        public decimal NetPrice { get; set; }
        public decimal DiscountPercentageValue { get; set; }

        public TaxRate TaxRate = new TaxRate();
        public long TaxRateID { get; set; }

        public decimal GrossPrice { get; set; }
        public decimal GrossPriceDiscountAmount { get; set; }

        public string OriginalOrderLineNo { get; set; }
        public decimal NetCost { get; set; }
        public decimal NetAmount { get; set; }
        public decimal NetPriceLessDiscount { get; set; }
        public decimal NetAmountLessDiscount { get; set; }

        public decimal Margin { get; set; }
    }
}
