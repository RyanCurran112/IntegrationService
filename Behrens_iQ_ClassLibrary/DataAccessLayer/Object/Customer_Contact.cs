/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Country
 *              Methods for Retreiving Countries via REST API
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
    class DAL_Customer_ContactJSONObject
    {
        public string TotalCount;
        public List<Customer_Contact> Data { get; set; }
    }
    class DAL_Customer_Contact
    {
        public Customer_Contact GetCustomer_Contact(long ID)
        {
            string apirequest = "CustomerContacts/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            Customer_Contact Customer_Contact = JsonSerializer.Deserialize<Customer_Contact>(restResponse.Content);

            Customer_Contact.CustomerID = Customer_Contact.Customer.ID;
            Customer_Contact.AddressID = Customer_Contact.Address.ID;

            return Customer_Contact;
        }

        public List<Customer_Contact> GetCustomer_Contacts(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "CustomerContacts?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_Customer_ContactJSONObject Customer_Contacts = JsonSerializer.Deserialize<DAL_Customer_ContactJSONObject>(restResponse.Content);

                List<Customer_Contact> listCustomer_Contacts = new List<Customer_Contact>();
                foreach (Customer_Contact customer_Contact in Customer_Contacts.Data)
                {
                    customer_Contact.CustomerID = customer_Contact.Customer.ID;
                    customer_Contact.AddressID = customer_Contact.Address.ID;

                    listCustomer_Contacts.Add(customer_Contact);
                }
                return listCustomer_Contacts;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Customer_Contact Customer_Contact = new Customer_Contact();
                List<Customer_Contact> listCustomer_Contacts = new List<Customer_Contact>() { Customer_Contact };
                return listCustomer_Contacts;
            }

        }

        public void CreateCustomer_Contact(Customer_Contact customer_Contact)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "CustomerContacts";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCustomer_Contact_APIString(customer_Contact), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    customer_Contact.ID = RestItems.id;
                }
            }
        }

        public void UpdateCustomer_Contact(Customer_Contact customer_Contact)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "CustomerContacts/" + customer_Contact.ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCustomer_Contact_APIString(customer_Contact), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    customer_Contact.ID = RestItems.id;
                }
            }
        }

        public void CreateCashCustomer_Contact(Customer_Contact customer_Contact)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "CustomerContacts2";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCashCustomer_Contact_APIString(customer_Contact), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    customer_Contact.ID = RestItems.id;
                }
            }
        }

        public void UpdateCashCustomer_Contact(Customer_Contact customer_Contact)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "CustomerContacts2/" + customer_Contact.ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCashCustomer_Contact_APIString(customer_Contact), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    customer_Contact.ID = RestItems.id;
                }
            }
        }

        public StringBuilder CreateCustomer_Contact_APIString(Customer_Contact customer_Contact)
        {
            StringBuilder APIString = new StringBuilder();

            APIString.Append("{");
            APIString.Append(" \"ID\":   \"" + customer_Contact._Owner_ + "\"");
            APIString.Append(",\"DeliveryContacts\":");
            APIString.Append("[{");
            APIString.Append("\"FirstName\":         \"" + customer_Contact.UniqueID + "\"");
            APIString.Append(",\"D_ContactName\":    \"" + customer_Contact.D_ContactName + "\"");
            APIString.Append(",\"Address\":          \"" + customer_Contact.AddressID + "\"");

            if (customer_Contact.EmailAddress != null) { APIString.Append(",\"EmailAddress\":     \"" + customer_Contact.EmailAddress + "\""); }
            if (customer_Contact.Phone != null) { APIString.Append(",\"Phone\":            \"" + customer_Contact.Phone + "\""); }
            if (customer_Contact.Mobile != null) { APIString.Append(",\"Mobile\":           \"" + customer_Contact.Mobile + "\""); }

            APIString.Append("}]");
            APIString.Append("}");

            return APIString;
        }

        public StringBuilder CreateCashCustomer_Contact_APIString(Customer_Contact customer_Contact)
        {
            StringBuilder APIString = new StringBuilder();
            APIString.Append("{");
            APIString.Append("\"FirstName\":         \"" + customer_Contact.UniqueID + "\"");
            APIString.Append(",\"D_ContactName\":    \"" + customer_Contact.D_ContactName + "\"");
            APIString.Append(",\"Address\":          \"" + customer_Contact.AddressID + "\"");

            if (customer_Contact.EmailAddress != null) { APIString.Append(",\"EmailAddress\":     \"" + customer_Contact.EmailAddress + "\""); }
            if (customer_Contact.Phone != null) { APIString.Append(",\"Phone\":            \"" + customer_Contact.Phone + "\""); }
            if (customer_Contact.Mobile != null) { APIString.Append(",\"Mobile\":           \"" + customer_Contact.Mobile + "\""); }

            APIString.Append(",\"LookupCustomer\":   \"" + customer_Contact.LookupCustomer + "\"");
            APIString.Append("}");

            return APIString;
        }
    }
}
