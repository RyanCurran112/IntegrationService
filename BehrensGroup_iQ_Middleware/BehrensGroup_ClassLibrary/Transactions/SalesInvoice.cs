/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object SalesInvoice
 *              Methods for Retreiving, Updating & Adding Sales Invoices via REST API
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
    public class SalesInvoiceJSONObject
    {
        public string TotalCount;
        public List<SalesInvoice> Data { get; set; }
    }

    public class SalesInvoice : GenericClassTransaction
    {
        //ID                    - GenericClassID
        //Number                - GenericClassTransactionNumber
        //CreatedDate           - GenericClassTransactionNumber
        //AlternateReference    - GenericClassTransaction
        //Particulars           - GenericClassTransaction

        public SalesPromotion SalesPromotion = new SalesPromotion();

        public SalesInvoice GetSalesInvoice(long ID)
        {
            string apirequest = "SalesInvoices/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            SalesInvoice SalesInvoice = JsonConvert.DeserializeObject<SalesInvoice>(restResponse.Content);

            return SalesInvoice;
        }

        public List<SalesInvoice> GetSalesInvoices(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "SalesInvoices?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                SalesInvoiceJSONObject SalesInvoices = JsonConvert.DeserializeObject<SalesInvoiceJSONObject>(restResponse.Content);

                List<SalesInvoice> listSalesInvoices = new List<SalesInvoice>();
                foreach (SalesInvoice SalesInvoice in SalesInvoices.Data)
                {
                    listSalesInvoices.Add(SalesInvoice);
                }
                return listSalesInvoices;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                SalesInvoice SalesInvoice = new SalesInvoice();

                List<SalesInvoice> listSalesInvoice = new List<SalesInvoice>
                {
                    SalesInvoice
                };
                return listSalesInvoice;
            }
        }

        public void UpdateSalesInvoice()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/" + "SalesInvoices/" + ID);
            //Method method = Method.PUT;

            //string apirequest = 

            var request = new RestRequest(Method.PUT);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", UpdateSalesInvoice_APIString(), ParameterType.RequestBody);

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

        public StringBuilder UpdateSalesInvoice_APIString()
        {
            StringBuilder APIString = new StringBuilder();

            APIString.Append("{\"Promotion\":\"" + SalesPromotion.Code + "\"}");

            return APIString;
        }

    }
}
