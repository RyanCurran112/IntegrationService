/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Customer
 *              Methods for Retreiving Customers via REST API
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
    public class CustomerJSONObject
    {
        public string TotalCount;
        public List<Customer> Data { get; set; }
    }

    public class Customer : GenericClassID
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID

        public string Code { get; set; }
        public string Name { get; set; }
        public string D_TradingNamePrefix { get; set; }
        public string TradingName { get; set; }
        public DateTime AccountOpenedOn { get; set; }

        //Contact Info
        public string Phone { get; set; }
        public string EmailAddress { get; set; }
        public string Homepage { get; set; }
        public string D_Instagram { get; set; }
        public string D_Facebook { get; set; }
        public string D_Twitter { get; set; }

        public IndustryType IndustryType = new IndustryType();
        public CustomerType Type = new CustomerType();
        public long TypeID { get; set; }
        public Division D_Division = new Division();

        //Location
        public Address Address = new Address();
        public Region Region = new Region();


        public static Customer GetCustomer(long ID)
        {
            string apirequest = "Customers/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            Customer Customer = JsonConvert.DeserializeObject<Customer>(restResponse.Content);

            return Customer;
        }

        public Customer GetCustomerCode(string htmlparameter)
        {
            try
            {
                string apirequest = "Customers?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                CustomerJSONObject Customers = JsonConvert.DeserializeObject<CustomerJSONObject>(restResponse.Content);

                List<Customer> listCustomer = new List<Customer>();
                foreach (Customer customer in Customers.Data)
                {
                    TypeID = customer.Type.ID;

                    listCustomer.Add(customer);
                    
                }
                return listCustomer[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<Customer> GetCustomers(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "Customers?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                CustomerJSONObject Customers = JsonConvert.DeserializeObject<CustomerJSONObject>(restResponse.Content);

                List<Customer> listCustomers = new List<Customer>();
                foreach (Customer Customer in Customers.Data)
                {
                    listCustomers.Add(Customer);
                }
                return listCustomers;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Customer Customer = new Customer() { ID = 0 };
                List<Customer> listCustomers = new List<Customer> { Customer };
                return listCustomers;
            }

        }
    }
}
