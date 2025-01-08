/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Customer.Contacts
 *              Methods for Retreiving, Updating & Adding Customer Contacts via REST API
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
    public class Customer_ContactJSONObject
    {
        public string TotalCount;
        public List<Customer_Contact> Data { get; set; }
    }

    public class Customer_Contact : GenericClassID
    {
        //ID                - GenericClassID
        //CreatedOn         - GenericClassID
        //CreatedBy         - GenericClassID
        //UpdatedOn         - GenericClassID
        //UpdatedBy         - GenericClassID

        public string UniqueID { get; set; }                //Unique ID is email address
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string D_ContactName { get; set; }

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
        public long CustomerID { get; set; }

        public Address Address = new Address();
        public long AddressID { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }

        public string ContactCompanyCode { get; set; }

        public string CustomerContactRoleCode { get; set; }
        public string EmailMarketingAllowed { get; set; }
        public string _Owner_ { get; set; }
        public long LookupCustomer { get; set; }
         

        public Customer_Contact GetCustomer_Contact(long ID)
        {
            string apirequest = "CustomerContacts/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            Customer_Contact Customer_Contact = JsonConvert.DeserializeObject<Customer_Contact>(restResponse.Content);

            CustomerID = Customer.ID;
            AddressID = Address.ID;

            return Customer_Contact;
        }

        public List<Customer_Contact> GetCustomer_Contacts(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "CustomerContacts?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                Customer_ContactJSONObject Customer_Contacts = JsonConvert.DeserializeObject<Customer_ContactJSONObject>(restResponse.Content);

                List<Customer_Contact> listCustomer_Contacts = new List<Customer_Contact>();
                foreach (Customer_Contact customer_Contact in Customer_Contacts.Data)
                {
                    CustomerID = customer_Contact.Customer.ID;
                    AddressID = customer_Contact.Address.ID;

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

        public void CreateCustomer_Contact()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "CustomerContacts";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCustomer_Contact_APIString(), ParameterType.RequestBody);

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

        public void UpdateCustomer_Contact()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "CustomerContacts/" + ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCustomer_Contact_APIString(), ParameterType.RequestBody);

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

        public void CreateCashCustomer_Contact()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "CustomerContacts2";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCashCustomer_Contact_APIString(), ParameterType.RequestBody);

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

        public void UpdateCashCustomer_Contact()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "CustomerContacts2/" + ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCashCustomer_Contact_APIString(), ParameterType.RequestBody);

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

        public StringBuilder CreateCustomer_Contact_APIString()
        {
            StringBuilder APIString = new StringBuilder();

            APIString.Append("{");
            APIString.Append(       " \"ID\":   \"" + _Owner_ + "\"");
            APIString.Append(       ",\"DeliveryContacts\":");
            APIString.Append(       "[{");
            APIString.Append(                               "\"FirstName\":         \"" + UniqueID + "\"");
            APIString.Append(                               ",\"D_ContactName\":    \"" + D_ContactName + "\"");
            APIString.Append(                               ",\"Address\":          \"" + AddressID + "\"");

            if (EmailAddress != null)   {APIString.Append(  ",\"EmailAddress\":     \"" + EmailAddress + "\""); }
            if (Phone != null)          {APIString.Append(  ",\"Phone\":            \"" + Phone + "\"");        }
            if (Mobile != null)         {APIString.Append(  ",\"Mobile\":           \"" + Mobile + "\"");       }

            APIString.Append(       "}]");
            APIString.Append("}");

            return APIString;
        }

        public StringBuilder CreateCashCustomer_Contact_APIString()
        {
            StringBuilder APIString = new StringBuilder();
            APIString.Append("{");
            APIString.Append(                               "\"FirstName\":         \"" + UniqueID + "\"");
            APIString.Append(                               ",\"D_ContactName\":    \"" + D_ContactName + "\"");
            APIString.Append(                               ",\"Address\":          \"" + AddressID + "\"");

            if (EmailAddress != null)   {APIString.Append(  ",\"EmailAddress\":     \"" + EmailAddress + "\""); }
            if (Phone != null)          {APIString.Append(  ",\"Phone\":            \"" + Phone + "\"");        }
            if (Mobile != null)         { APIString.Append( ",\"Mobile\":           \"" + Mobile + "\"");       }
            
            APIString.Append(                               ",\"LookupCustomer\":   \"" + LookupCustomer + "\"");
            APIString.Append("}");

            return APIString;
        }

    }
}
