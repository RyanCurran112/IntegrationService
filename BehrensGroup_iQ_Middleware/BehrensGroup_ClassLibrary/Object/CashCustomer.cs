/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object CashCustomer
 *              Methods for Retreiving, Updating & Adding Cash Customers via REST API
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
    public class CashCustomerJSONObject
    {
        public string TotalCount;
        public List<CashCustomer> Data { get; set; }
    }

    public class CashCustomer : GenericClassObject
    {
        //ID                        - GenericClassID
        //CreatedOn                 - GenericClassID
        //UpdatedOn                 - GenericClassID

        //Code                      - GenericClassObject
        //Description               - GenericClassObject

        private string firstname;
        public string FirstName
        {
            get { return firstname; }
            set
            {
                if (value != null)
                {
                    firstname = value.Replace(",", ".");
                    firstname = StringHelper.ToTitleCase(firstname);
                }
            }
        }

        private string lastname;
        public string LastName
        {
            get { return lastname; }
            set
            {
                if (value != null)
                {
                    lastname = value.Replace(",", ".");
                    lastname = StringHelper.ToTitleCase(lastname);
                }
            }
        }

        public string FullName
        {
            get { return lastname; }
            set
            {
                if (value != null)
                {
                    lastname = value.Replace(",", ".");
                    lastname = StringHelper.ToTitleCase(lastname);
                }
            }
        }


        //public string FullName => $"{FirstName} {LastName}";

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

        private string emailaddress;
        public string EmailAddress //UniqueID is cash customers email address
        { 
            get { return emailaddress; }
            set { if (value != null) { emailaddress = value.Replace(",", "."); } }
        }

        private string phone;
        public string Phone 
        { 
            get { return phone; }
            set { if (value != null) {  phone = value.Replace(",", ".").Replace("#", "").Replace(" ",""); } } 
        }

        private string mobile;
        public string Mobile
        {
            get { return mobile; }
            set { if (value != null) { mobile = value.Replace(",", ".").Replace("#", ""); } }
        }

        public Branch Branch = new Branch();
        public long BranchID;

        public Division D_Division = new Division();
        public long D_DivisionID { get; set; }

        public Address Address = new Address();
        public long AddressID { get; set; }

        public Salutation Salutation = new Salutation();
        public List<long> CashCustomer_Contacts = new List<long>();
        
        public static CashCustomer GetCashCustomer(long ID)
        {
            string apirequest = "CashCustomers/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            CashCustomer CashCustomer = JsonConvert.DeserializeObject<CashCustomer>(restResponse.Content);

            return CashCustomer;
        }
        
        public List<CashCustomer> GetCashCustomers(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            { 
                string apirequest = "CashCustomers?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                CashCustomerJSONObject CashCustomers = JsonConvert.DeserializeObject<CashCustomerJSONObject>(restResponse.Content);

                List<CashCustomer> listCashCustomer = new List<CashCustomer>();
                foreach (CashCustomer cashCustomer in CashCustomers.Data)
                {
                    AddressID = cashCustomer.Address.ID;
                    BranchID = cashCustomer.Branch.ID;
                    D_DivisionID = cashCustomer.D_Division.ID;

                    listCashCustomer.Add(cashCustomer);
                }
                
                return listCashCustomer;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                CashCustomer CashCustomer = new CashCustomer();
                List<CashCustomer> listCashCustomer = new List<CashCustomer>() { CashCustomer };
                return listCashCustomer;
            }
        }

        public void CreateCashCustomer()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "CashCustomers";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCashCustomer_APIString(), ParameterType.RequestBody);

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

        public void UpdateCashCustomer()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "CashCustomers/" + ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateCashCustomer_APIString(), ParameterType.RequestBody);

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

        public StringBuilder CreateCashCustomer_APIString()
        {
            StringBuilder APIString = new StringBuilder();
            APIString.Append("{");
            APIString.Append(                       " \"FirstName\":    \"" + EmailAddress + "\"");
            APIString.Append(                       ",\"LastName\":     \"" + FullName + "\"");
            //APIString.Append(                     ",\"Salutation\":   \"" + Salutation + "\"");
            APIString.Append(                       ",\"EmailAddress\": \"" + EmailAddress + "\"");
            APIString.Append(                       ",\"D_Division\":   \"" + D_DivisionID + "\"");

            if (Phone != null)  {APIString.Append(  ",\"Phone\":        \"" + Phone + "\"");    }
            if (Mobile != null) {APIString.Append(  ",\"Mobile\":       \"" + Mobile + "\"");   }

            APIString.Append(                       ",\"Branch\":       \"" + Branch.ID + "\"");
            APIString.Append(                       ",\"Address\":      \"" + AddressID + "\"");
            APIString.Append("}");
            return APIString;
        }
    }
}
