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

namespace BehrensGroup_ClassLibrary.Object
{
    public class Customer_ProductPropertiesJSONObject
    {
        public string TotalCount;
        public List<Customer_ProductProperties> Data { get; set; }
    }

    public class Customer_ProductProperties : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID

        public string CustomerProductCode { get; set; }
        public string CustomerProductDescription { get; set; }

        public Product Product = new Product();
        public Customer _Owner_ = new Customer();

        public long ProductID { get; set; }
        public long CustomerID { get; set; }
        public string CustomerCode { get; set; }

        public Customer_ProductProperties GetCustomer_ProductProperties(long ID)
        {
            string apirequest = "CustomerProductProperties/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            Customer_ProductProperties Customer_ProductProperties = JsonConvert.DeserializeObject<Customer_ProductProperties>(restResponse.Content);

            CustomerID = _Owner_.ID;
            CustomerCode = _Owner_.Code;
            ProductID = Product.ID;

            return Customer_ProductProperties;
        }

        public List<Customer_ProductProperties> GetCustomer_ProductProperties(string htmlparameter)
        {
            try
            {
                string apirequest = "CustomerProductProperties?CustomerProductCode[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                Customer_ProductPropertiesJSONObject Customer_ProductPropertiess = JsonConvert.DeserializeObject<Customer_ProductPropertiesJSONObject>(restResponse.Content);

                List<Customer_ProductProperties> listCustomer_ProductProperties = new List<Customer_ProductProperties>();
                foreach (Customer_ProductProperties customer_ProductProperties in Customer_ProductPropertiess.Data)
                {
                    customer_ProductProperties.Product = Product.GetProductCode(customer_ProductProperties.Product.Code);
                    listCustomer_ProductProperties.Add(customer_ProductProperties);
                }
                return listCustomer_ProductProperties;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
