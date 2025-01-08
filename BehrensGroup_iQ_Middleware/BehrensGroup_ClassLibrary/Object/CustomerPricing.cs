/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object CashCustomer
 *              Methods for Retreiving, Updating & Adding Cash Customers via REST API
 */

using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using RestSharp;

using BehrensGroup_ClassLibrary.BaseClasses;
using BehrensGroup_ClassLibrary.Functions;

namespace BehrensGroup_ClassLibrary.Object
{
    public class CustomerPricingJSONObject
    {
        public CustomerPricing CustomerPricing = new CustomerPricing();
    }

    public class CustomerPricing
    {
        public double BaseNetPrice { get; set; }
        public double NetPrice { get; set; }
        public double BaseGrossPrice { get; set; }
        public double GrossPrice { get; set; }
        public double DiscountPercentage { get; set; }
        public double AdditionalDiscountPercentage1 { get; set; }
        public double AdditionalDiscountPercentage2 { get; set; }
        public double AdditionalDiscountPercentage3 { get; set; }
        public double EMCAmount { get; set; }

        public ProductProperties ProductProperties = new ProductProperties();
        public QuantityBreakInfos QuantityBreakInfos = new QuantityBreakInfos();

        public CustomerPricing GetCustomerPricing(string CustomerCode, string ProductCode, string Quantity)
        {
            try
            {
                CustomerPricing FDBE = new CustomerPricing();

                var definition = new { Product = "" };

                string apirequest = "CustomerPricing?customer=" + CustomerCode + "&products=" + ProductCode + "&quantity=" + Quantity;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                var CustomerPricing = JsonConvert.DeserializeAnonymousType(restResponse.Content, definition);

                CustomerPricing CustomerPricings = new CustomerPricing();

                return CustomerPricings;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

    }

    public class ProductProperties
    {

    }
    public class QuantityBreakInfos
    {

    }
}
