/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Product
 *              Methods for Retreiving Products via REST API
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
    class DAL_ProductJSONObject
    {
        public string TotalCount;
        public List<Product> Data { get; set; }
    }

    class DAL_Product
    {
        public Product GetProduct(long ID)
        {
            string apirequest = "Products/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            Product Product = JsonSerializer.Deserialize<Product>(restResponse.Content);

            return Product;
        }

        public Product GetProductCode(string htmlparameter)
        {
            try
            {
                string apirequest = "Products?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_ProductJSONObject Products = JsonSerializer.Deserialize<DAL_ProductJSONObject>(restResponse.Content);

                List<Product> listProduct = new List<Product>();
                foreach (Product product in Products.Data)
                {
                    listProduct.Add(product);
                }
                return listProduct[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<Product> GetProducts(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "Products?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_ProductJSONObject Products = JsonSerializer.Deserialize<DAL_ProductJSONObject>(restResponse.Content);

                List<Product> listProducts = new List<Product>();
                foreach (Product Product in Products.Data)
                {
                    listProducts.Add(Product);
                }
                return listProducts;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Product Product = new Product();
                List<Product> listProducts = new List<Product>() { Product };
                return listProducts;
            }

        }
    }
}
