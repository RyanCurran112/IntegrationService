using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using RestSharp;

using BehrensGroup_ClassLibrary.Functions;

namespace BehrensGroup_ClassLibrary.Object
{
    public class CashCustomer_Contact : Contact
    {
        public string UniqueID { get; set; }                //Unique ID is email address
        

        public _Salutation Salutation;
        public enum _Salutation
        {
            Mr,
            Mrs,
            Dr,
            Ms,
            Miss
        }

        public Customer Customer = new Customer();
        public Address Address = new Address();


        public class Customer_ContactJSONObject
        {
            public string TotalCount;
            public List<Customer_Contact> Data { get; set; }
        }

        public static void GetCashCustomer_Contact(long ID)
        {
            string apirequest = "Customer_Contact/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            Customer_Contact Customer_Contact = JsonConvert.DeserializeObject<Customer_Contact>(restResponse.Content);

        }

        public static void GetCashCustomer_Contacts(string htmloperator, string htmlattribute, string htmlparameter)
        {
            string apirequest = "Customer_Contact?" + htmlattribute + htmloperator + "=" + htmlparameter;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            Customer_ContactJSONObject rootobject = JsonConvert.DeserializeObject<Customer_ContactJSONObject>(restResponse.Content);

        }
        public void CreateCashCustomer_Contact()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "Customer_Contact";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            string AddressString = Address.AddressLine1 + "," + Address.AddressLine2 + "," + Address.City + "," + Address.County + "," + Address.Country.Code + "," + Address.PostCode;

            request.AddParameter("undefined", "{\n\t\"FirstName\": \"" + EmailAddress + "\",\n\t\"LastName\": \"" + FullName + "\",\n\t\"Salutation\": \"" + Salutation + "\",\n\t\"EmailAddress\": \"" + EmailAddress + "\",\n\t\"Phone\": \"" + Phone + "\",\n\t\"Mobile\": \"" + Mobile + "\",\n\t\"Branch\": \"HQ\",\n\t\"Address\": \"" + AddressString + "\"\n}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
        }
        public void UpdateCashCustomer_Contact()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            long ID = 10522710662303;

            string apirequest = "Customer_Contact/" + ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            string AddressString = Address.AddressLine1 + "," + Address.AddressLine2 + "," + Address.City + "," + Address.County + "," + Address.Country.Code + "," + Address.PostCode;
            request.AddParameter("undefined", "{\n\t\"FirstName\": \"" + UniqueID + "\",\n\t\"LastName\": \"" + FullName + "\",\n\t\"Salutation\": \"" + Salutation + "\",\n\t\"EmailAddress\": \"" + EmailAddress + "\",\n\t\"Phone\": \"" + Phone + "\",\n\t\"Mobile\": \"" + Mobile + "\",\n\t\"Branch\": \"HQ\",\n\t\"Address\": \"" + AddressString + "\"\n}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
        }

    }
}
