/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Data Access Layer for Object Customer
 *              Methods for Retreiving, Updating & Adding Customers via REST API
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
    class DAL_CustomerJSONObject
    {
        public string TotalCount;
        public List<Customer> Data { get; set; }
    }

    class DAL_Customer
    {
        public Customer GetCustomer(long ID)
        {
            string apirequest = "Customers/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            Customer Customer = JsonSerializer.Deserialize<Customer>(restResponse.Content);

            return Customer;
        }

        public Customer GetCustomerCode(string htmlparameter)
        {
            try
            {
                string apirequest = "Customers?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_CustomerJSONObject Customers = JsonSerializer.Deserialize<DAL_CustomerJSONObject>(restResponse.Content);

                List<Customer> listCustomer = new List<Customer>();
                foreach (Customer customer in Customers.Data)
                {
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
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_CustomerJSONObject Customers = JsonSerializer.Deserialize<DAL_CustomerJSONObject>(restResponse.Content);

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
                Customer Customer = new Customer();

                List<Customer> listCustomers = new List<Customer>(){Customer};
                return listCustomers;
            }

        }
    }
}
