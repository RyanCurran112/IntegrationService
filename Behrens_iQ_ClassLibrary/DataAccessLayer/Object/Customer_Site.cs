/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Data Access Layer for Object Customer Site
 *              Methods for Retreiving, Updating, Adding Customer Site via REST API
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
    class DAL_Customer_SiteJSONObject
    {
        public string TotalCount;
        public List<Customer_Site> Data { get; set; }
    }

    class DAL_Customer_Site
    {
        public Customer_Site GetCustomer_Site(long ID)
        {
            string apirequest = "CustomerSites/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            Customer_Site Customer_Site = JsonSerializer.Deserialize<Customer_Site>(restResponse.Content);

            return Customer_Site;
        }

        public List<Customer_Site> GetCustomer_Sites(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "CustomerSites?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_Customer_SiteJSONObject Customer_Sites = JsonSerializer.Deserialize<DAL_Customer_SiteJSONObject>(restResponse.Content);

                List<Customer_Site> listCustomer_Sites = new List<Customer_Site>();
                foreach (Customer_Site customer_Site in Customer_Sites.Data)
                {
                    listCustomer_Sites.Add(customer_Site);
                }
                return listCustomer_Sites;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Customer_Site Customer_Site = new Customer_Site();
                Customer_Site.ID = 0;
                List<Customer_Site> listCustomer_Sites = new List<Customer_Site>();

                listCustomer_Sites.Add(Customer_Site);
                return listCustomer_Sites;
            }

        }
        public void CreateCustomer_Site(Customer_Site customer_Site)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "CustomerSites";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCustomer_Site_APIString(customer_Site), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    customer_Site.ID = RestItems.id;
                }
            }
        }

        public void UpdateCustomer_Site(Customer_Site customer_Site)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "CustomerSites/" + customer_Site.ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCustomer_Site_APIString(customer_Site), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    customer_Site.ID = RestItems.id;
                }
            }
        }

        public StringBuilder CreateCustomer_Site_APIString(Customer_Site customer_Site)
        {
            StringBuilder APIString = new StringBuilder();

            APIString.Append("{");
            APIString.Append("\"ID\":\"" + customer_Site._Owner_ + "\"");
            APIString.Append(",\"Sites\": ");
            APIString.Append("[");
            APIString.Append("{\"Code\": \"" + customer_Site.Code + "\"");
            APIString.Append(",\"Description\": \"" + customer_Site.Description + "\"}");
            APIString.Append("]}");

            return APIString;
        }
    }
}
