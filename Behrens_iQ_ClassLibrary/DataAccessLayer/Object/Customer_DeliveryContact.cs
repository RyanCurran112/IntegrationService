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
    class DAL_Customer_DeliveryContactJSONObject
    {
        public string TotalCount;
        public List<Customer_DeliveryContact> Data { get; set; }
    }

    class DAL_Customer_DeliveryContact
    {
        public Customer_DeliveryContact GetCustomer_DeliveryContact(long ID)//uses API to get the information using the unique customer id and then adds the data to the object's attributes
        {
            string apirequest = "CustomerDeliveryContacts2/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            Customer_DeliveryContact customer_DeliveryContact = JsonSerializer.Deserialize<Customer_DeliveryContact>(restResponse.Content);

            customer_DeliveryContact.AddressID = customer_DeliveryContact.Address.ID;
            customer_DeliveryContact.CustomerID = customer_DeliveryContact.Customer.ID;
            customer_DeliveryContact.CashCustomerID = customer_DeliveryContact.CashCustomer.ID;

            return customer_DeliveryContact;
        }

        public List<Customer_DeliveryContact> GetCustomer_DeliveryContacts(List<BEH_RESTFilter> filterstrings)//returns list of customer delivery contacts
        {
            try
            {
                int i = 0;
                string htmlstring = "";

                foreach (BEH_RESTFilter filterstring in filterstrings)//goes through the list and constructs the html based on the values of each item in the list
                {
                    htmlstring += filterstring.htmlAttribute + filterstring.htmlOperator + "=" + filterstring.htmlParameter;

                    if (i == 0) { htmlstring += "&"; }
                    i++;
                }

                string apirequest = "CustomerDeliveryContacts2?" + htmlstring;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_Customer_DeliveryContactJSONObject Customer_DeliveryContacts = JsonSerializer.Deserialize<DAL_Customer_DeliveryContactJSONObject>(restResponse.Content);

                List<Customer_DeliveryContact> listCustomer_DeliveryContacts = new List<Customer_DeliveryContact>();
                foreach (Customer_DeliveryContact customer_DeliveryContact in Customer_DeliveryContacts.Data)
                {
                    customer_DeliveryContact.AddressID = customer_DeliveryContact.Address.ID;
                    customer_DeliveryContact.CustomerID = customer_DeliveryContact.Customer.ID;
                    customer_DeliveryContact.CashCustomerID = customer_DeliveryContact.CashCustomer.ID;

                    listCustomer_DeliveryContacts.Add(customer_DeliveryContact);//adds information to list and returns the completed list at the end
                }
                return listCustomer_DeliveryContacts;
            }
            catch (Exception e)//e is the error
            {
                Console.WriteLine(e.Message);
                Customer_DeliveryContact Customer_DeliveryContact = new Customer_DeliveryContact();

                List<Customer_DeliveryContact> listCustomer_DeliveryContacts = new List<Customer_DeliveryContact>{ Customer_DeliveryContact };

                return listCustomer_DeliveryContacts;
            }

        }
        public void CreateCustomer_DeliveryContact(Customer_DeliveryContact customer_DeliveryContact)//creates a new delivery contact for a non cach customer
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;//Post creates a new object, in this case the customer delivery contact
            string apirequest = "CustomerDeliveryContacts";//object for non cash customers

            var request = new RestRequest(apirequest, method);//method from rest library(?)
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCustomer_DeliveryContact_APIString(customer_DeliveryContact), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    customer_DeliveryContact.ID = RestItems.id;
                }
            }
        }

        public void UpdateCustomer_DeliveryContact(Customer_DeliveryContact customer_DeliveryContact)//updates delivery contact of non cash customer
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;//PUT updates an eexisting object, in this case a customer delivery contact

            string apirequest = "CustomerDeliveryContacts/" + customer_DeliveryContact.ID;//goes to that particular cdc object

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCustomer_DeliveryContact_APIString(customer_DeliveryContact), ParameterType.RequestBody);//updates contact

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    customer_DeliveryContact.ID = RestItems.id;
                }
            }
        }

        public void CreateCashCustomer_DeliveryContact(Customer_DeliveryContact customer_DeliveryContact)//creates 
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "CustomerDeliveryContacts2";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCashCustomer_DeliveryContact_APIString(customer_DeliveryContact), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    customer_DeliveryContact.ID = RestItems.id;
                }
            }
        }

        public void UpdateCashCustomer_DeliveryContact(Customer_DeliveryContact customer_DeliveryContact)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "CustomerDeliveryContacts2/" + customer_DeliveryContact.ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCashCustomer_DeliveryContact_APIString(customer_DeliveryContact), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    customer_DeliveryContact.ID = RestItems.id;
                }
            }
        }

        public StringBuilder CreateCustomer_DeliveryContact_APIString(Customer_DeliveryContact customer_DeliveryContact)
        {
            StringBuilder APIString = new StringBuilder();

            APIString.Append("{");
            APIString.Append("\"ID\":                    \"" + customer_DeliveryContact._Owner_ + "\"");
            APIString.Append(",\"DeliveryContacts\":");
            APIString.Append("[{");
            APIString.Append(" \"FirstName\":        \"" + customer_DeliveryContact.UniqueID1 + "\"");
            APIString.Append(",\"LastName\":         \"" + customer_DeliveryContact.UniqueID2 + "\"");
            APIString.Append(",\"D_ContactName\":    \"" + customer_DeliveryContact.D_ContactName + "\"");
            APIString.Append(",\"CashCustomer\":     \"" + customer_DeliveryContact.CashCustomerID + "\"");
            APIString.Append(",\"Address\":          \"" + customer_DeliveryContact.AddressID + "\"");

            if (customer_DeliveryContact.EmailAddress != null) { APIString.Append(",\"EmailAddress\":     \"" + customer_DeliveryContact.EmailAddress + "\""); }
            if (customer_DeliveryContact.Phone != null) { APIString.Append(",\"Phone\":            \"" + customer_DeliveryContact.Phone + "\""); }
            if (customer_DeliveryContact.Mobile != null) { APIString.Append(",\"Mobile\":           \"" + customer_DeliveryContact.Mobile + "\""); }

            APIString.Append("}]");
            APIString.Append("}");
            return APIString;
        }

        public StringBuilder CreateCashCustomer_DeliveryContact_APIString(Customer_DeliveryContact customer_DeliveryContact)
        {
            StringBuilder APIString = new StringBuilder();
            APIString.Append("{");
            APIString.Append(" \"FirstName\":         \"" + customer_DeliveryContact.UniqueID1 + "\"");
            APIString.Append(",\"LastName\":         \"" + customer_DeliveryContact.UniqueID2 + "\"");
            APIString.Append(",\"D_ContactName\":    \"" + customer_DeliveryContact.D_ContactName + "\"");
            APIString.Append(",\"CashCustomer\":     \"" + customer_DeliveryContact.CashCustomerID + "\"");
            APIString.Append(",\"Address\":          \"" + customer_DeliveryContact.AddressID + "\"");

            if (customer_DeliveryContact.EmailAddress != null) { APIString.Append(",\"EmailAddress\":     \"" + customer_DeliveryContact.EmailAddress + "\""); }
            if (customer_DeliveryContact.Phone != null) { APIString.Append(",\"Phone\":            \"" + customer_DeliveryContact.Phone + "\""); }
            if (customer_DeliveryContact.Mobile != null) { APIString.Append(",\"Mobile\":           \"" + customer_DeliveryContact.Mobile + "\""); }

            APIString.Append(",\"LookupCustomer\":   \"" + customer_DeliveryContact.LookupCustomerID + "\"");
            APIString.Append("}");

            return APIString;
        }
    }
}
