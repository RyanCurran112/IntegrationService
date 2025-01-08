/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Data Access Layer for Object Cash Customer
 *              Methods for Retreiving, Updating & Adding Cash Customers via REST API
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using RestSharp;

using Behrens_iQ_ClassLibrary.BaseClasses;
using Behrens_iQ_ClassLibrary.Functions;
using Behrens_iQ_ClassLibrary.Models.Object;

namespace Behrens_iQ_ClassLibrary.DataAccessLayer.Object
{
    class DAL_CashCustomerJSONObject
    {
        public string TotalCount;
        public List<CashCustomer> Data { get; set; }
    }

    class DAL_CashCustomer
    {
        public CashCustomer GetCashCustomer(long ID)
        {
            string apirequest = "CashCustomers/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            CashCustomer CashCustomer = JsonSerializer.Deserialize<CashCustomer>(restResponse.Content);

            CashCustomer.AddressID = CashCustomer.Address.ID;

            return CashCustomer;
        }

        public List<CashCustomer> GetCashCustomers(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "CashCustomers?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_CashCustomerJSONObject CashCustomers = JsonSerializer.Deserialize<DAL_CashCustomerJSONObject>(restResponse.Content);

                List<CashCustomer> listCashCustomer = new List<CashCustomer>();
                foreach (CashCustomer cashCustomer in CashCustomers.Data)
                {
                    cashCustomer.AddressID = cashCustomer.Address.ID;
                    cashCustomer.BranchID = cashCustomer.Branch.ID;
                    cashCustomer.D_DivisionID = cashCustomer.D_Division.ID;

                    listCashCustomer.Add(cashCustomer);
                }

                return listCashCustomer;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                CashCustomer CashCustomer = new CashCustomer();
                List<CashCustomer> listCashCustomer = new List<CashCustomer>(){   CashCustomer  };

                return listCashCustomer;
            }
        }

        public void CreateCashCustomer(CashCustomer CashCustomer)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "CashCustomers";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCashCustomer_APIString(CashCustomer), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    CashCustomer.ID = RestItems.id;
                }
            }
        }

        public void UpdateCashCustomer(CashCustomer CashCustomer)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "CashCustomers/" + CashCustomer.ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCashCustomer_APIString(CashCustomer), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    CashCustomer.ID = RestItems.id;
                }
            }
        }

        public StringBuilder CreateCashCustomer_APIString(CashCustomer CashCustomer)
        {
            StringBuilder APIString = new StringBuilder();
            APIString.Append("{");
            APIString.Append(" \"FirstName\":    \"" + CashCustomer.EmailAddress + "\"");
            APIString.Append(",\"LastName\":     \"" + CashCustomer.FullName + "\"");
            APIString.Append(",\"Salutation\":   \"" + CashCustomer.Salutation + "\"");
            APIString.Append(",\"EmailAddress\": \"" + CashCustomer.EmailAddress + "\"");

            if (CashCustomer.Phone != null) { APIString.Append(",\"Phone\":        \"" + CashCustomer.Phone + "\""); }
            if (CashCustomer.Mobile != null) { APIString.Append(",\"Mobile\":       \"" + CashCustomer.Mobile + "\""); }

            APIString.Append(",\"Branch\":       \"" + CashCustomer.Branch.ID + "\"");
            APIString.Append(",\"Address\":      \"" + CashCustomer.Address.ID + "\"");
            APIString.Append("}");
            return APIString;
        }
    }
}
