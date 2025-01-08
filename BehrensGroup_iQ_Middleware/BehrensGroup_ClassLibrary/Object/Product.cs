/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Product
 *              Methods for Retreiving Products via REST API
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
  
    public class ProductJSONObject : GenericJSONObject
    {
        //TotalCount    - GenericClassJSONObject;
        public List<Product> Data { get; set; }
    }

    public class Product : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        //Code          - GenericClassObject
        //Description   - GenericClassObject
       
        public ProductType Type = new ProductType();
        public string StrippedCode { get; set; }
        public long CategoryID { get; set; }
        public long ProductID { get; set; }
        public ProductCategory Category = new ProductCategory();

        public Product_Selling Selling = new Product_Selling();

        public List<Product_AssemblyRevision> Product_AssemblyRevisions = new List<Product_AssemblyRevision>();

        public SupersededBy SupersededBy = new SupersededBy();
        public SupersededFrom SupersededFrom = new SupersededFrom();

        public Product GetProduct(long ID)
        {
            string apirequest = "Products/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            Product Product = JsonConvert.DeserializeObject<Product>(restResponse.Content);

            return Product;
        }

        public Product GetProductCode(string htmlparameter)
        {
            try
            {
                string apirequest = "Products?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                ProductJSONObject Products = JsonConvert.DeserializeObject<ProductJSONObject>(restResponse.Content);

                List<Product> listProduct = new List<Product>();
                
                foreach (Product product in Products.Data)
                {
                    Product productNew = new Product();

                    ProductStatistics ProductStatistics = new ProductStatistics();
                    ProductStatistics = ProductStatistics.GetProductStatistics(product.ID);

                    if(product.SupersededBy.ID != 0 && ProductStatistics.P_HQ_BranchStatistics.UnallocatedStockLevel == 0)
                    {
                        productNew = product.GetProductCode(product.SupersededBy.Code);
                    }
                    else { productNew = product; }

                    listProduct.Add(productNew);
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
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                ProductJSONObject Products = JsonConvert.DeserializeObject<ProductJSONObject>(restResponse.Content);

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
    public class SupersededBy : GenericClassObject
    {
    }

    public class SupersededFrom : GenericClassObject
    {
    }
}
