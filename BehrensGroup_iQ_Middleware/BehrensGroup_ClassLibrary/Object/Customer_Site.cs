/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object CashCustomer
 *              Methods for Retreiving, Updating & Adding Customer Sites via REST API
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
    public class Customer_SiteJSONObject
    {
        public string TotalCount;
        public List<Customer_Site> Data { get; set; }
    }

    public class Customer_Site : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //CreatedBy     - GenericClassID
        //UpdatedOn     - GenericClassID
        //UpdatedBy     - GenericClassID
        //Code          - GenericClassObject
        //Description   - GenericClassObject

        public string _Owner_ { get; set; }

        public Customer_Site GetCustomer_Site(long ID)
        {
            string apirequest = "CustomerSites/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            Customer_Site Customer_Site = JsonConvert.DeserializeObject<Customer_Site>(restResponse.Content);

            return Customer_Site;
        }

        public List<Customer_Site> GetCustomer_Sites(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "CustomerSites?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                Customer_SiteJSONObject Customer_Sites = JsonConvert.DeserializeObject<Customer_SiteJSONObject>(restResponse.Content);

                List<Customer_Site> listCustomer_Sites = new List<Customer_Site>();
                foreach (Customer_Site Customer_Site in Customer_Sites.Data)
                {
                    listCustomer_Sites.Add(Customer_Site);
                }
                return listCustomer_Sites;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Customer_Site Customer_Site = new Customer_Site() { ID = 0 };
                List<Customer_Site> listCustomer_Sites = new List<Customer_Site> { Customer_Site };
                return listCustomer_Sites;
            }

        }
        public void CreateCustomer_Site()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "CustomerSites";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCustomer_Site_APIString(), ParameterType.RequestBody);

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

        public void UpdateCustomer_Site()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "CustomerSites/" + ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCustomer_Site_APIString(), ParameterType.RequestBody);

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

        public StringBuilder CreateCustomer_Site_APIString()
        {
            StringBuilder APIString = new StringBuilder();

            APIString.Append("{");
            APIString.Append("\"ID\":\"" + _Owner_ + "\"");
            APIString.Append(",\"Sites\": ");
            APIString.Append("[");
            APIString.Append("{\"Code\": \"" + Code + "\"");
            APIString.Append(",\"Description\": \"" + Description + "\"}");
            APIString.Append("]}");

            return APIString;
        }

    }
}
