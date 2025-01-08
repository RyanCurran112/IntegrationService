/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Data Access Layer for Object Cash Customer Contacts
 *              Methods for Retreiving, Updating & Adding Cash Customer Contacts via REST API
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
    class DAL_CashCustomer_ContactJSONObject
    {
        public string TotalCount;
        public List<Customer_Contact> Data { get; set; }
    }

    class DAL_CashCustomer_Contact
    {
        public static void GetCashCustomer_Contact(long ID)
        {
            string apirequest = "Customer_Contact/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            Customer_Contact Customer_Contact = JsonSerializer.Deserialize<Customer_Contact>(restResponse.Content);
        }

        public void GetCashCustomer_Contacts(string htmloperator, string htmlattribute, string htmlparameter)
        {
            string apirequest = "Customer_Contact?" + htmlattribute + htmloperator + "=" + htmlparameter;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            DAL_Customer_ContactJSONObject rootobject = JsonSerializer.Deserialize<DAL_Customer_ContactJSONObject>(restResponse.Content);

        }
        public void CreateCashCustomer_Contact(CashCustomer_Contact cashCustomer_Contact)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "Customer_Contact";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            //string AddressString = cashCustomer_Contact.Address.AddressLine1 + "," + cashCustomer_Contact.Address.AddressLine2 + "," + cashCustomer_Contact.Address.City + "," + cashCustomer_Contact.Address.County + "," + Address.Country.Code + "," + Address.PostCode;

            //request.AddParameter("undefined", "{\n\t\"FirstName\": \"" + cashCustomer_Contact.EmailAddress + "\",\n\t\"LastName\": \"" + cashCustomer_Contact.FullName + "\",\n\t\"Salutation\": \"" + Salutation + "\",\n\t\"EmailAddress\": \"" + EmailAddress + "\",\n\t\"Phone\": \"" + Phone + "\",\n\t\"Mobile\": \"" + Mobile + "\",\n\t\"Branch\": \"HQ\",\n\t\"Address\": \"" + AddressString + "\"\n}", ParameterType.RequestBody);

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

            //string AddressString = Address.AddressLine1 + "," + Address.AddressLine2 + "," + Address.City + "," + Address.County + "," + Address.Country.Code + "," + Address.PostCode;
            //request.AddParameter("undefined", "{\n\t\"FirstName\": \"" + UniqueID + "\",\n\t\"LastName\": \"" + FullName + "\",\n\t\"Salutation\": \"" + Salutation + "\",\n\t\"EmailAddress\": \"" + EmailAddress + "\",\n\t\"Phone\": \"" + Phone + "\",\n\t\"Mobile\": \"" + Mobile + "\",\n\t\"Branch\": \"HQ\",\n\t\"Address\": \"" + AddressString + "\"\n}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
        }
    }
}
