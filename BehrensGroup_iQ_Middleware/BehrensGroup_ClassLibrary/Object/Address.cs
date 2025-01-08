/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Address
 *              Methods for Retreiving, Updating & Adding Addresses via REST API
 */

using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using RestSharp;

using BehrensGroup_ClassLibrary.BaseClasses;
using BehrensGroup_ClassLibrary.Functions;
using System.Globalization;

namespace BehrensGroup_ClassLibrary.Object
{
    public class Address : GenericClassID
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //CreatedBy     - GenericClassID
        //UpdatedOn     - GenericClassID
        //UpdatedBy     - GenericClassID
        
        private string addressline1;
        public string AddressLine1 
        { 
            get { return addressline1; }
            set
            { 
                if (value != null) 
                { 
                    addressline1 = value.Replace(",", ".");
                    addressline1 = StringHelper.ToTitleCase(addressline1);
                } 
            } 
        }

        private string addressline2;
        public string AddressLine2
        {
            get { return addressline2; }
            set
            {
                if (value != null)
                {
                    addressline2 = value.Replace(",", ".");
                    addressline2 = StringHelper.ToTitleCase(addressline2);
                }
            }
        }

        private string addressline3;
        public string AddressLine3
        {
            get { return addressline3; }
            set
            {
                if (value != null)
                {
                    addressline3 = value.Replace(",", ".");
                    addressline3 = StringHelper.ToTitleCase(addressline3);
                }
            }
        }

        private string addressline4;
        public string AddressLine4
        {
            get { return addressline4; }
            set
            {
                if (value != null)
                {
                    addressline4 = value.Replace(",", ".");
                    addressline4 = StringHelper.ToTitleCase(addressline4);
                }
            }
        }

        public string Town { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public Country Country = new Country();

        private string postcode;
        public string PostCode 
         {
            get { return postcode; }
            set { if (value != null) { postcode = value.Replace(",", ".").ToUpper(); } }
        }
        

        //API Methods
        public static Address GetAddress(long ID)
        {
            string apirequest = "Addresses/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            Address Address = JsonConvert.DeserializeObject<Address>(restResponse.Content);
            
            return Address;
        }

        public static string GetAddresses(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "Addresses?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                CashCustomerJSONObject Addresses = JsonConvert.DeserializeObject<CashCustomerJSONObject>(restResponse.Content);
                return Addresses.TotalCount;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "0";
            }
        }

        public long CreateAddress()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "Addresses";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            long ID = 0;

            request.AddParameter("undefined", CreateAddress_APIString(), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            RESTReponse RestResponse = JsonConvert.DeserializeObject<RESTReponse>(response.Content);

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

        public void UpdateAddress()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "Addresses/" + ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateAddress_APIString(), ParameterType.RequestBody);

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

        public StringBuilder CreateAddress_APIString()
        {
            StringBuilder APIString = new StringBuilder();
            APIString.Append("{\"AddressLine1\": \"" + AddressLine1 + "\"");
            APIString.Append(",\"AddressLine2\": \"" + AddressLine2 + "\"");
            APIString.Append(",\"AddressLine3\": \"" + AddressLine3 + "\"");
            APIString.Append(",\"AddressLine4\": \"" + AddressLine4 + "\"");

            if (Country.Code == "GB")
            {
                Country.Code = "UK";
            }

            APIString.Append(",\"Country\": \"" + Country.Code + "\"");
            APIString.Append(",\"PostCode\": \"" + PostCode + "\"}");

            return APIString;
        }
    }
}
