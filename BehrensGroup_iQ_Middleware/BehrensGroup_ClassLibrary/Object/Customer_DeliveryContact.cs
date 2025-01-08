/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Customer.DeliveryContacts
 *              Methods for Retreiving, Updating & Adding Customer Delivery Contacts via REST API
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using Newtonsoft.Json;
using RestSharp;

using BehrensGroup_ClassLibrary.BaseClasses;
using BehrensGroup_ClassLibrary.Functions;

namespace BehrensGroup_ClassLibrary.Object
{
    public class Customer_DeliveryContactJSONObject
    {
        public string TotalCount;
        public List<Customer_DeliveryContact> Data { get; set; }
    }

    public class Customer_DeliveryContact : GenericClassID
    {
        //ID                        - GenericClassID
        //CreatedOn                 - GenericClassID
        //UpdatedOn                 - GenericClassID

        

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }

        //UniqueID1 is delivery contacts email address
        private string uniqueid1;
        public string UniqueID1 
        {
            get { return uniqueid1; }
            set { if (value != null) { uniqueid1 = value.Replace(",", "."); } }
        }

        //UniqueID2 is delivery contacts postcode with spacing/characters removed.
        private string uniqueid2;
        public string UniqueID2 
        {
            get { return uniqueid2; }
            set { if (value != null) { uniqueid2 = value.Replace(",", ".").ToUpper().Replace(" ", ""); } }
        }               

        private string d_contactname;
        public string D_ContactName
        {
            get { return d_contactname; }
            set
            {
                if (value != null)
                {
                    d_contactname = value.Replace(",", ".");
                    d_contactname = StringHelper.ToTitleCase(d_contactname);
                }
            }
        }

        public _Salutation Salutation;
        public enum _Salutation
        {
            Mr,
            Mrs,
            Dr,
            Ms,
            Miss
        }

        private string companyname;
        public string CompanyName 
        {
            get { return companyname; }
            set
            {
                if (value != null)
                {
                    companyname = value.Replace(",", ".");
                    companyname = StringHelper.ToTitleCase(companyname);
                }
            }

        }

        public Customer LookupCustomer = new Customer();
        public long LookupCustomerID { get; set; }

        public Customer Customer = new Customer();
        public string _Owner_;
        public long CustomerID { get; set; }

        public CashCustomer CashCustomer = new CashCustomer();
        public long CashCustomerID { get; set; }

        public Address Address = new Address();
        public long AddressID { get; set; }

        public DeliveryAgent DefaultDeliveryAgent = new DeliveryAgent();
        public long DefaultDeliveryAgentID { get; set; }

        public DeliveryAgent_Service DefaultDeliveryAgent_Service = new DeliveryAgent_Service();
        public long DefaultDeliveryAgentServiceID { get; set; }


        //Contact Details
        private string emailaddress;
        public string EmailAddress 
        {
            get { return emailaddress; }
            set { if (value != null) { emailaddress = value.Replace(",", "."); } } 
        }

        private string phone;
        public string Phone 
        {
            get {return phone; }
            set { if (value != null) { phone = value.Replace(",", ".").Replace("#", "").Replace(" ", "").TrimStart(); } }
        }
        private string mobile;
        public string Mobile 
        {
            get { return mobile; }
            set { if (value != null) { mobile = value.Replace(",", ".").Replace("#", ""); } }
        }
        
        

        //API Methods
        public Customer_DeliveryContact GetCustomer_DeliveryContact(long ID)
        {
            string apirequest = "CustomerDeliveryContacts2/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            Customer_DeliveryContact Customer_DeliveryContact = JsonConvert.DeserializeObject<Customer_DeliveryContact>(restResponse.Content);

            AddressID = Address.ID;
            CustomerID = Customer.ID;
            CashCustomerID = CashCustomer.ID;

            return Customer_DeliveryContact;
        }

        public List<Customer_DeliveryContact> GetCustomer_DeliveryContacts(List<RESTFilter> filterstrings)
        {
            try
            {
                int i = 0;
                string htmlstring = "";

                foreach (RESTFilter filterstring in filterstrings)
                {
                    htmlstring += filterstring.htmlAttribute + filterstring.htmlOperator + "=" + filterstring.htmlParameter;
                    if (i == 0) { htmlstring += "&"; }

                    i++;
                }

                string apirequest = "CustomerDeliveryContacts2?" + htmlstring;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                Customer_DeliveryContactJSONObject Customer_DeliveryContacts = JsonConvert.DeserializeObject<Customer_DeliveryContactJSONObject>(restResponse.Content);

                List<Customer_DeliveryContact> listCustomer_DeliveryContacts = new List<Customer_DeliveryContact>();
                foreach (Customer_DeliveryContact customer_DeliveryContact in Customer_DeliveryContacts.Data)
                {
                    AddressID = customer_DeliveryContact.Address.ID;
                    CustomerID = customer_DeliveryContact.Customer.ID;
                    CashCustomerID = customer_DeliveryContact.CashCustomer.ID;
                    LookupCustomerID = customer_DeliveryContact.LookupCustomer.ID;

                    listCustomer_DeliveryContacts.Add(customer_DeliveryContact);
                }
                return listCustomer_DeliveryContacts;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Customer_DeliveryContact Customer_DeliveryContact = new Customer_DeliveryContact();

                List<Customer_DeliveryContact> listCustomer_DeliveryContacts = new List<Customer_DeliveryContact>
                {
                    Customer_DeliveryContact
                };

                return listCustomer_DeliveryContacts;
            }

        }
        public void CreateCustomer_DeliveryContact()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "CustomerDeliveryContacts";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCustomer_DeliveryContact_APIString(), ParameterType.RequestBody);

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

        public void UpdateCustomer_DeliveryContact()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "CustomerDeliveryContacts/" + ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCustomer_DeliveryContact_APIString(), ParameterType.RequestBody);

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

        public void CreateCashCustomer_DeliveryContact()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "CustomerDeliveryContacts2";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCashCustomer_DeliveryContact_APIString(), ParameterType.RequestBody);

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

        public void UpdateCashCustomer_DeliveryContact()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "CustomerDeliveryContacts2/" + ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCashCustomer_DeliveryContact_APIString(), ParameterType.RequestBody);

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

        public StringBuilder CreateCustomer_DeliveryContact_APIString()
        {
            StringBuilder APIString = new StringBuilder();

                                                            APIString.Append("{");
                                                            APIString.Append("\"ID\":                               \"" + _Owner_ + "\"");
                                                            APIString.Append(",\"DeliveryContacts\":");
                                                            APIString.Append("[{");

            if (!String.IsNullOrEmpty(UniqueID1))       {   APIString.Append(" \"FirstName\":                       \"" + UniqueID1 + "\"");         }
            if (!String.IsNullOrEmpty(UniqueID2))       {   APIString.Append(",\"LastName\":                        \"" + UniqueID2 + "\"");         }
            if (!String.IsNullOrEmpty(D_ContactName))   {   APIString.Append(",\"D_ContactName\":                   \"" + D_ContactName + "\"");     }
            if (CashCustomerID != 0)                    {   APIString.Append(",\"CashCustomer\":                    \"" + CashCustomerID + "\"");    }
            if (AddressID != 0)                         {   APIString.Append(",\"Address\":                         \"" + AddressID + "\"");         }

            if (EmailAddress != null)                   {   APIString.Append(",\"EmailAddress\":                    \"" + EmailAddress + "\"");      }
            if (Phone != null)                          {   APIString.Append(",\"Phone\":                           \"" + Phone + "\"");             }
            if (Mobile != null)                         {   APIString.Append(",\"Mobile\":                          \"" + Mobile + "\"");            }


            if (DefaultDeliveryAgentID != 0)            {   APIString.Append(",\"D_DefaultDeliveryAgent\":          \"" + DefaultDeliveryAgentID + "\""); }
            if (DefaultDeliveryAgentServiceID != 0)     {   APIString.Append(",\"D_DefaultDeliveryAgentService\":   \"" + DefaultDeliveryAgentServiceID + "\""); }

                                                            APIString.Append("}]");
                                                            APIString.Append("}");

            return APIString;
        }

        public StringBuilder CreateCashCustomer_DeliveryContact_APIString()
        {
            StringBuilder APIString = new StringBuilder();
                                                            
                                                            APIString.Append("{");
            
                                                            APIString.Append(" \"FirstName\":                       \"" + UniqueID1 + "\"");        
            if (!String.IsNullOrEmpty(UniqueID2))       {   APIString.Append(",\"LastName\":                        \"" + UniqueID2 + "\"");        }
            if (!String.IsNullOrEmpty(D_ContactName))   {   APIString.Append(",\"D_ContactName\":                   \"" + D_ContactName + "\"");    }
            if (CashCustomerID != 0)                    {   APIString.Append(",\"CashCustomer\":                    \"" + CashCustomerID + "\"");   }
            if (AddressID != 0)                         {   APIString.Append(",\"Address\":                         \"" + AddressID + "\"");        }

            if (EmailAddress != null)                   {   APIString.Append(",\"EmailAddress\":                    \"" + EmailAddress + "\"");     }
            if (Phone != null)                          {   APIString.Append(",\"Phone\":                           \"" + Phone + "\"");            }
            if (Mobile != null)                         {   APIString.Append(",\"Mobile\":                          \"" + Mobile + "\"");           }
            
            if (DefaultDeliveryAgentID != 0)            {   APIString.Append(" \"D_DefaultDeliveryAgent\":          \"" + DefaultDeliveryAgentID + "\"");  }
            if (DefaultDeliveryAgentServiceID != 0)     {   APIString.Append(",\"D_DefaultDeliveryAgentService\":   \"" + DefaultDeliveryAgentServiceID + "\""); }
            if (LookupCustomerID != 0)                  {   APIString.Append(",\"LookupCustomer\":                  \"" + LookupCustomerID + "\""); }

                                                            APIString.Append("}");

            return APIString;
        }

    }
}
