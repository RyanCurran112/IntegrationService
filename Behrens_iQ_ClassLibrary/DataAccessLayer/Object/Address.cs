/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Data Access Layer for Object Address
 *              Methods for Retreiving, Updating & Adding Addresses via REST API
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
    class DAL_AddressJSONObject
    {
        public string TotalCount;
        public List<Address> Data { get; set; }
    }

    class DAL_Address
    {
        public Address GetAddress(long ID)
        {
            string apirequest = "Addresses/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            Address Address = JsonSerializer.Deserialize<Address>(restResponse.Content);

            return Address;
        }

        public int GetAddresses(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "Addresses?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_AddressJSONObject Addresses = JsonSerializer.Deserialize<DAL_AddressJSONObject>(restResponse.Content);

                return Convert.ToInt32(Addresses.TotalCount);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        public long CreateAddress(Address Address)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "Addresses";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            long ID = 0;

            request.AddParameter("undefined", CreateAddress_APIString(Address), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    ID = RestItems.id;
                }
                return ID;
            }
            else
            {
                return 0;
            }
        }

        public void UpdateAddress(Address Address)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "Addresses/" + Address.ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateAddress_APIString(Address), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    Address.ID = RestItems.id;
                }
            }
        }

        public StringBuilder CreateAddress_APIString(Address Address)
        {
            StringBuilder APIString = new StringBuilder();

            APIString.Append("{\"AddressLine1\": \"" + Address.AddressLine1 + "\"");
            APIString.Append(",\"AddressLine2\": \"" + Address.AddressLine2 + "\"");
            APIString.Append(",\"AddressLine3\": \"" + Address.AddressLine3 + "\"");
            APIString.Append(",\"AddressLine4\": \"" + Address.AddressLine4 + "\"");

            if (Address.Country.Code == "GB")   {   Address.Country.Code = "UK";    }

            APIString.Append(",\"Country\": \"" + Address.Country.Code + "\"");
            APIString.Append(",\"PostCode\": \"" + Address.PostCode + "\"}");

            return APIString;
        }
    }
}
