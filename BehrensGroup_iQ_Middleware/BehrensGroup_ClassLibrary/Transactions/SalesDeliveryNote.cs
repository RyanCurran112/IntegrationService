/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object SalesDeliveryNote
 *              Methods for Retreiving, Updating & Adding Sales Delivery Notes via REST API
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
    public class SalesDeliveryNoteJSONObject
    {
        public string TotalCount;
        public List<SalesDeliveryNote> Data { get; set; }
    }

    public class SalesDeliveryNote : GenericClassTransaction
    {
        //ID                    - GenericClassID
        //Number                - GenericClassTransactionNumber
        //CreatedDate           - GenericClassTransactionNumber
        //AlternateReference    - GenericClassTransaction
        //Particulars           - GenericClassTransaction

        public Branch Branch = new Branch();
        public Branch DispatchBranch = new Branch();
        public Division D_Division = new Division();
        public string AlternateReference { get; set; }
        public string DueDate { get; set; }

        public Customer Customer = new Customer();
        public CashCustomer CashCustomer = new CashCustomer();
        public Customer_DeliveryContact DeliveryContact = new Customer_DeliveryContact();
        public Customer_Contact Contact = new Customer_Contact();
        public Customer_Site Site = new Customer_Site();
        public DeliveryAgent DeliveryAgent = new DeliveryAgent();
        public DeliveryAgent_Service DeliveryAgentService = new DeliveryAgent_Service();
        public string DeliveryAgentTrackingNumber;
        public string DeliveryPricingType { get; set; }
        public decimal DeliveryPriceNet { get; set; }
        public TaxRate DeliveryTaxRate = new TaxRate();
        public decimal DeliveryPriceGross { get; set; }
        public string ConsignmentNo { get; set; }

        public Currency Currency = new Currency();
        public SalesRep SalesRep = new SalesRep();
        public SalesPromotion SalesPromotion = new SalesPromotion();
        public decimal NetCost { get; set; }
        public decimal NetAmount { get; set; }
        public decimal NetAmountLessDiscount { get; set; }
        public decimal Margin { get; set; }

        public List<SalesDeliveryNote_Line> SalesDeliveryNoteLines = new List<SalesDeliveryNote_Line>();

        public SalesDeliveryNote GetSalesDeliveryNote(long ID)
        {
            string apirequest = "SalesDeliveryNotes/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            SalesDeliveryNote SalesDeliveryNote = JsonConvert.DeserializeObject<SalesDeliveryNote>(restResponse.Content);

            return SalesDeliveryNote;
        }

        public List<SalesDeliveryNote> GetSalesDeliveryNotes(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "SalesDeliveryNotes?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                SalesDeliveryNoteJSONObject SalesDeliveryNotes = JsonConvert.DeserializeObject<SalesDeliveryNoteJSONObject>(restResponse.Content);

                List<SalesDeliveryNote> listSalesDeliveryNotes = new List<SalesDeliveryNote>();
                foreach (SalesDeliveryNote SalesDeliveryNote in SalesDeliveryNotes.Data)
                {
                    listSalesDeliveryNotes.Add(SalesDeliveryNote);
                }
                return listSalesDeliveryNotes;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                SalesDeliveryNote SalesDeliveryNote = new SalesDeliveryNote();

                List<SalesDeliveryNote> listSalesDeliveryNote = new List<SalesDeliveryNote>
                {
                    SalesDeliveryNote
                };
                return listSalesDeliveryNote;
            }
        }

        public void UpdateSalesDeliveryNote()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/" + "SalesDeliveryNotes/" + ID);
            //Method method = Method.PUT;

            //string apirequest = 

            var request = new RestRequest(Method.PUT);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", UpdateSalesDeliveryNote_APIString(), ParameterType.RequestBody);

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

        public StringBuilder UpdateSalesDeliveryNote_APIString()
        {
            StringBuilder APIString = new StringBuilder();

            //"{DeliveryAgentTrackingNumber\":\"1ZR5E5856843730226\"\n}\n"

            APIString.Append("{\"DeliveryAgentTrackingNumber\":\"" + DeliveryAgentTrackingNumber + "\"}");
            //APIString.Append("{\"Promotion\":\"" + SalesPromotion.Code + "\"}");

            return APIString;
        }
    }
}
