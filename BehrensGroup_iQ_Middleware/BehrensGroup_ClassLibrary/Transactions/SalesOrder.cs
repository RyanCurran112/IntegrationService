/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object SalesOrder
 *              Methods for Retreiving, Updating & Adding Sales Orders via REST API
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
    public class SalesOrderJSONObject
    {
        public string TotalCount;
        public List<SalesOrder> Data { get; set; }
    }

    public class SalesOrder : GenericClassTransaction
    {
        //ID                    - GenericClassID
        //Number                - GenericClassTransactionNumber
        //CreatedDate           - GenericClassTransactionNumber

        public string AlternateReference { get; set; }

        public Branch Branch = new Branch();
        public Branch DespatchBranch = new Branch();
        public Division D_Division = new Division();
        public SalesRep SalesRep = new SalesRep();
        public SalesOrderType OrderType = new SalesOrderType();
        public SalesOrderSource Source = new SalesOrderSource();
        public SalesOrderWorkflowStatus WorkflowStatus = new SalesOrderWorkflowStatus();
        
        public Customer Customer = new Customer();
        public CashCustomer CashCustomer = new CashCustomer();
        public Customer_DeliveryContact DeliveryContact = new Customer_DeliveryContact();
        public Customer_Contact Contact = new Customer_Contact();
        public Customer_Site Site = new Customer_Site();

        public DeliveryAgent DeliveryAgent = new DeliveryAgent();
        public DeliveryAgent_Service DeliveryAgentService = new DeliveryAgent_Service();
        public DeliveryInstructions DeliveryInstructions = new DeliveryInstructions();
        public TaxRate DeliveryTaxRate = new TaxRate();
        public SalesPromotion Promotion = new SalesPromotion();

        //Order Information
        private string date;
        public string Date
        {
            get { return date; }
            set { date = Convert.ToDateTime(value).ToString().Replace("/", "-").Replace(":", "-"); }
        }

        private string duedate;
        public string DueDate
        {
            get { return duedate; }
            set { duedate = Convert.ToDateTime(value).ToString().Replace("/", "-").Replace(":", "-"); }
        }
        public string DateAnticipated { get; set; }

        public Currency Currency = new Currency();
        public decimal TransactionExchangeRate { get; set; }
        public decimal AccountExchangeRate { get; set; }
        
        public string DeliveryPricingType { get; set; }
        public decimal DeliveryNetAmount { get; set; }
        public decimal DeliveryCostAmount { get; set; }
        public decimal DeliveryGrossAmount { get; set; }
        public decimal DeliveryPriceGross { get; set; }
        public string AdditionalDeliveryNotes { get; set; }
        public string ConsignmentNo { get; set; }

        public string Particulars { get; set; }
        public string PricingType { get; set; }
        public int SaleAgreementType { get; set; }
        public string D_TagNumber { get; set; }
        //NHS Requisition Numbers - Used to identify which department purchased goods
        private string d_reqpt1;
        public string D_ReqPt1
        {
            get { return d_reqpt1; }
            set { if (value != null) { d_reqpt1 = value.Replace("Reqpt Name:", ""); } }
        }
        private string d_reqpt2;
        public string D_ReqPt2
        {
            get { return d_reqpt2; }
            set { if (value != null) { d_reqpt2 = value.Replace("Reqpt:", ""); } }
        }

        public string D_ReqPt => $"{D_ReqPt1}. {D_ReqPt2}";

        private string d_reqno;
        public string D_ReqNo
        {
            get { return d_reqno; }
            set { if (value != null) {  d_reqno = value.Replace("Reqptno:", ""); } }
        }

        public string D_EmbroideryCode { get; set; }

        public string OrderPricingType { get; set; }
        public decimal NetCost { get; set; }
        public decimal NetAmount { get; set; }
        public decimal NetAmountLessDiscount { get; set; }
        public decimal Margin { get; set; }
        
        public List<SalesOrder_Line> SalesOrderLines = new List<SalesOrder_Line>();
        public List<Receipt> SalesOrderReceipts = new List<Receipt>();


        public SalesOrder GetSalesOrder(long ID)
        {
            string apirequest = "SalesOrders/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            SalesOrder SalesOrder = JsonConvert.DeserializeObject<SalesOrder>(restResponse.Content);

            return SalesOrder;
        }

        public List<SalesOrder> GetSalesOrders(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "SalesOrders?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                SalesOrderJSONObject SalesOrders = JsonConvert.DeserializeObject<SalesOrderJSONObject>(restResponse.Content);

                List<SalesOrder> listSalesOrders = new List<SalesOrder>();
                foreach (SalesOrder SalesOrder in SalesOrders.Data)
                {
                    listSalesOrders.Add(SalesOrder);
                }
                return listSalesOrders;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                SalesOrder SalesOrder = new SalesOrder();

                List<SalesOrder> listSalesOrder = new List<SalesOrder>
                {
                    SalesOrder
                };
                return listSalesOrder;
            }
        }

        public void CreateSalesOrder()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "SalesOrders";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateSalesOrder_APIString(), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            
            RESTReponse RestResponse = JsonConvert.DeserializeObject<RESTReponse>(response.Content);

             if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    ID = RestItems.id;
                }
            }
        }

        public void UpdateSalesOrder()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "SalesOrders/" + ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", UpdateSalesOrder_APIString(), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonConvert.DeserializeObject<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    ID = RestItems.id;
                }
            }

        }

        public StringBuilder CreateSalesOrder_APIString()
        {
            StringBuilder APIString = new StringBuilder();

            decimal TotalValue;

            APIString.Append("{");
            APIString.Append(                                           " \"AlternateReference\":       \"" + AlternateReference + "\"");
            //APIString.Append(                                           " \"DueDate\":                  \"" + DueDate + "\"");
            APIString.Append(                                           ",\"OrderType\":                \"" + OrderType.ID + "\"");
            APIString.Append(                                           ",\"Customer\":                 \"" + Customer.ID + "\"");
            APIString.Append(                                           ",\"Branch\":                   \"" + Branch.ID + "\"");
            APIString.Append(                                           ",\"DespatchBranch\":           \"" + DespatchBranch.ID + "\"");
            if (CashCustomer.ID != 0)           { APIString.Append(     ",\"CashCustomer\":             \"" + CashCustomer.ID + "\""); }
            if (DeliveryContact.ID != 0)        { APIString.Append(     ",\"DeliveryContact\":          \"" + DeliveryContact.ID + "\""); }
            APIString.Append(                                           ",\"D_Division\":               \"" + D_Division.ID + "\"");
            if (Source.ID != 0)                 { APIString.Append(     ",\"Source\":                   \"" + Source.ID + "\""); }
            if (SalesRep.ID != 0)               { APIString.Append(     ",\"SalesRep\":                 \"" + SalesRep.ID + "\""); }
            APIString.Append(                                           ",\"Currency\":                 \"" + Currency.ID + "\"");
            APIString.Append(                                           ",\"TransactionExchangeRate\":  \"1\"");
            APIString.Append(                                           ",\"AccountExchangeRate\":      \"1\"");
            if (Promotion.ID != 0)              { APIString.Append(     ",\"Promotion\":                \"" + Promotion.ID + "\""); }

            if (DeliveryAgent.ID != 0)          { APIString.Append(     ",\"DeliveryAgent\":            \"" + DeliveryAgent.ID + "\""); }
            if (DeliveryAgentService.ID != 0)   { APIString.Append(     ",\"DeliveryAgentService\":     \"" + DeliveryAgentService.ID + "\""); }

            if (D_TagNumber != null)            { APIString.Append(     ",\"D_TagNumber\":              \"" + D_TagNumber + "\""); }
            if (D_EmbroideryCode != null)       { APIString.Append(     ",\"D_EmbroideryCode\":         \"" + D_EmbroideryCode + "\""); }

            if (D_ReqNo != null)                { APIString.Append(     ",\"D_ReqNo\":                  \"" + D_ReqNo + "\""); }
            if (D_ReqPt != null)                { APIString.Append(     ",\"D_ReqPt\":                  \"" + D_ReqPt + "\""); }
            if (Particulars != null)            { APIString.Append(     ",\"Particulars\":              \"" + Particulars + "\""); }

            APIString.Append(                                           ",\"DeliveryCostAmount\":       \"" + DeliveryCostAmount + "\"");

            if (DeliveryPricingType == "NetPricing")
            {
                APIString.Append(                                       ",\"DeliveryNetAmount\":        \"" + DeliveryNetAmount + "\"");
                if (DeliveryTaxRate.ID != 0) { APIString.Append(        ",\"DeliveryTaxRate\":          \"" + DeliveryTaxRate.ID + "\""); }
            }
            else if (DeliveryPricingType == "GrossPricing")
            {
                if (DeliveryTaxRate.ID != 0) { APIString.Append(        ",\"DeliveryTaxRate\":          \"" + DeliveryTaxRate.ID + "\""); }
                APIString.Append(                                       ",\"DeliveryGrossAmount\":      \"" + DeliveryGrossAmount + "\""); 
            }

            TotalValue = DeliveryGrossAmount;

            APIString.Append(",\"Items\": ");
            APIString.Append(       "[{");

            int i = 1;
            foreach(SalesOrder_Line line in SalesOrderLines)
            {
                if (i > 1)                          { APIString.Append(  ",{");                                                }

                APIString.Append(                                       " \"Product\":                  \"" + line.Product.ID + "\"");

                if (line.D_ContactName != null)     { APIString.Append( ",\"D_ContactName\":            \"" + line.D_ContactName + "\""); }
                if (line.D_Monogram != null)        { APIString.Append( ",\"D_Mono\":                   \"" + line.D_Monogram + "\""); }
                if (line.D_EmbroideryCode != null)  { APIString.Append( ",\"D_EmbroideryCode\":         \"" + line.D_EmbroideryCode + "\""); }

                APIString.Append(                                       ",\"Quantity\":                 \"" + line.Quantity + "\"");

                if (line.SellingUnits != null)      { APIString.Append( ",\"SellingUnits\":             \"" + line.SellingUnits.ID + "\""); }

                if (PricingType == "NetPricing")
                {
                    APIString.Append(                                   ",\"NetPrice\":                 \"" + line.NetPrice + "\"");
                    APIString.Append(                                   ",\"DiscountPercentageValue\":  \"" + line.DiscountPercentageValue + "\"");
                    APIString.Append(                                   ",\"TaxRate\":                  \"" + line.TaxRate.ID + "\"");
                }
                else if (PricingType == "GrossPricing")
                {
                    APIString.Append(                                   ",\"TaxRate\":                  \"" + line.TaxRate.ID + "\"");
                    APIString.Append(                                   ",\"GrossPrice\":               \"" + line.GrossPrice + "\"");
                    APIString.Append(                                   ",\"GrossPriceDiscountAmount\": \"" + line.GrossPriceDiscountAmount + "\"");
                }
                 
                APIString.Append(                                   "}");
                TotalValue += (line.GrossPrice - line.GrossPriceDiscountAmount)*line.Quantity;
                i++;
            }

            APIString.Append(       "]");
            
            if (SalesOrderReceipts.Count > 0)
            { 
                APIString.Append(",\"Receipts\": ");
                APIString.Append(       "[{");

                i = 1;
                foreach (Receipt receipt in SalesOrderReceipts)
                {
                    if (i > 1)      {   APIString.Append(           ",{");}

                    APIString.Append(                                   "\"ReceiptType\":               \"" + receipt.ReceiptType + "\"");
                    APIString.Append(                                   ",\"Amount\":                   \"" + receipt.Amount + "\"");
                    APIString.Append(                               "}");
                    i++;
                }
            
                APIString.Append(       "]");
            }
            

            APIString.Append(       "}");

            return APIString;
        }

        public static StringBuilder UpdateSalesOrder_APIString()
        {
            StringBuilder APIString = new StringBuilder();

            APIString.Append("{\"SaleAgreementType\":\"" + 5 + "\"}");
            //APIString.Append("{\"DueDate\":\"" + DueDate + "\"}");

            return APIString;
        }

        public decimal CalculateDeliveryNetPriceFromDeliveryGrossPrice()
        {
            if (DeliveryTaxRate.Code == "GB01" || DeliveryTaxRate.Code == "GB01N")
            {
                DeliveryNetAmount = DeliveryPriceGross / 1.2M;
            }
            else if (DeliveryTaxRate.Code == "GB02" || DeliveryTaxRate.Code == "GB02N")
            {
                DeliveryNetAmount = DeliveryPriceGross / 1.05M;
            }
            else
            {
                DeliveryNetAmount = DeliveryPriceGross;
            }
            return DeliveryNetAmount;
        }

        public decimal CalculateDeliveryGrossPriceFromDeliveryNetPrice()
        {
            if (DeliveryTaxRate.Code == "GB01" || DeliveryTaxRate.Code == "GB01N")
            {
                DeliveryPriceGross = DeliveryNetAmount * 1.2M;
            }
            else if (DeliveryTaxRate.Code == "GB02" || DeliveryTaxRate.Code == "GB02N")
            {
                DeliveryPriceGross = DeliveryNetAmount * 1.05M;
            }
            else
            {
                DeliveryPriceGross = DeliveryNetAmount;
            }
            return DeliveryPriceGross;
        }
    }


}
