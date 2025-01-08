/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Data Access Layer for Transaction Sales Order
 *              Methods for Retreiving, Updating & Adding Sales Orders via REST API
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using RestSharp;

using Behrens_iQ_ClassLibrary.BaseClasses;
using Behrens_iQ_ClassLibrary.Functions;
using Behrens_iQ_ClassLibrary.Models.Object;
using Behrens_iQ_ClassLibrary.Models.Transaction;

namespace Behrens_iQ_ClassLibrary.DataAccessLayer.Transaction
{
    class DAL_SalesOrderJSONObject
    {
        public string TotalCount;
        public List<DAL_SalesOrder> Data { get; set; }
    }
    class DAL_SalesOrder
    {
        public DAL_SalesOrder GetSalesOrder(long ID)
        {
            string apirequest = "SalesOrders/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            DAL_SalesOrder SalesOrder = JsonSerializer.Deserialize<DAL_SalesOrder>(restResponse.Content);

            return SalesOrder;
        }

        public List<DAL_SalesOrder> GetSalesOrders(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "SalesOrders?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_SalesOrderJSONObject SalesOrders = JsonSerializer.Deserialize<DAL_SalesOrderJSONObject>(restResponse.Content);

                List<DAL_SalesOrder> listSalesOrders = new List<DAL_SalesOrder>();
                foreach (DAL_SalesOrder SalesOrder in SalesOrders.Data)
                {
                    listSalesOrders.Add(SalesOrder);
                }
                return listSalesOrders;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                DAL_SalesOrder SalesOrder = new DAL_SalesOrder();

                List<DAL_SalesOrder> listSalesOrder = new List<DAL_SalesOrder>{ SalesOrder  };
                return listSalesOrder;
            }
        }

        public void CreateSalesOrder(SalesOrder salesOrder)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "SalesOrders";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateSalesOrder_APIString(salesOrder), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    salesOrder.ID = RestItems.id;
                }
            }
        }

        public void UpdateSalesOrder(SalesOrder salesOrder)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "SalesOrders/" + salesOrder.ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateSalesOrder_APIString(salesOrder), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    salesOrder.ID = RestItems.id;
                }
            }

        }

        public StringBuilder CreateSalesOrder_APIString(SalesOrder salesOrder)
        {
            StringBuilder APIString = new StringBuilder();

            APIString.Append("{");
            APIString.Append(" \"AlternateReference\":       \"" + salesOrder.AlternateReference + "\"");
            APIString.Append(",\"OrderType\":                \"" + salesOrder.OrderType.ID + "\"");
            APIString.Append(",\"Customer\":                 \"" + salesOrder.Customer.ID + "\"");
            APIString.Append(",\"Branch\":                   \"" + salesOrder.Branch.ID + "\"");
            APIString.Append(",\"DespatchBranch\":           \"" + salesOrder.DespatchBranch.ID + "\"");
            APIString.Append(",\"D_Division\":               \"" + salesOrder.D_Division.ID + "\"");
            APIString.Append(",\"Source\":                   \"" + salesOrder.Source.ID + "\"");
            APIString.Append(",\"SalesRep\":                 \"" + salesOrder.SalesRep.ID + "\"");
            APIString.Append(",\"Currency\":                 \"" + salesOrder.Currency.ID + "\"");
            APIString.Append(",\"TransactionExchangeRate\":  \"1\"");
            APIString.Append(",\"AccountExchangeRate\":      \"1\"");

            if (salesOrder.DeliveryPricingType == "NetPricing")
            {
                APIString.Append(",\"DeliveryNetAmount\":        \"" + salesOrder.DeliveryNetAmount + "\"");
                APIString.Append(",\"DeliveryTaxRate\":          \"" + salesOrder.DeliveryTaxRate + "\"");
            }
            else if (salesOrder.DeliveryPricingType == "GrossPricing")
            {
                APIString.Append(",\"DeliveryTaxRate\":          \"" + salesOrder.DeliveryTaxRate + "\"");
                APIString.Append(",\"DeliveryGrossAmount\":      \"" + salesOrder.DeliveryPriceGross + "\"");
            }

            APIString.Append(",\"Items\": ");
            APIString.Append("[{");

            int i = 1;
            foreach (SalesOrder_Line line in salesOrder.SalesOrderLines)
            {
                if (i > 1) { APIString.Append(",{"); }

                APIString.Append("\"Product\":                   \"" + line.Product + "\"");

                if (line.D_ContactName != null) { APIString.Append(",\"D_ContactName\":             \"" + line.D_ContactName + "\""); }
                if (line.D_Monogram != null) { APIString.Append(",\"D_Mono\":                       \"" + line.D_Monogram + "\""); }
                if (line.D_EmbroideryCode != null) { APIString.Append(",\"D_EmbroideryCode\":       \"" + line.D_EmbroideryCode + "\""); }

                APIString.Append(",\"Quantity\":                 \"" + line.Quantity + "\"");

                if (line.SellingUnits != null) { APIString.Append(",\"SellingUnits\":             \"" + line.SellingUnits + "\""); }

                if (salesOrder.OrderPricingType == "NetPricing")
                {
                    APIString.Append("\"NetPrice\":                  \"" + line.NetPrice + "\"");
                    APIString.Append("\"DiscountPercentageValue\":   \"" + line.DiscountPercentageValue + "\"");
                    APIString.Append("\"TaxRate\":                   \"" + line.TaxRate + "\"");
                }
                else if (salesOrder.OrderPricingType == "GrossPricing")
                {
                    APIString.Append("\"TaxRate\":                   \"" + line.TaxRate + "\"");
                    APIString.Append("\"GrossPrice\":                \"" + line.GrossPrice + "\"");
                    APIString.Append("\"GrossPriceDiscountAmount\":  \"" + line.GrossPriceDiscountAmount + "\"");
                }

                APIString.Append("}");
                i++;
            }

            APIString.Append("]");

            APIString.Append(",\"Receipts\": ");
            APIString.Append("[{");

            foreach (Receipt receipt in salesOrder.SalesOrderReceipts)
            {
                if (i > 1) { APIString.Append(",{"); }

                APIString.Append("\"ReceiptType\":               \"" + receipt.ReceiptType + "\"");
                APIString.Append(",\"Amount\":                   \"" + receipt.Amount + "\"");
                APIString.Append("}");
                i++;
            }

            APIString.Append("]}");

            return APIString;
        }
    }
}
