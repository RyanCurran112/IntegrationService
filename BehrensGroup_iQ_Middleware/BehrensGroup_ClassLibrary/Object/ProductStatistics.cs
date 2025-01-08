/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Product
 *              Methods for Retreiving Product Statistics via REST API
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
    public class ProductStatisticsJSONObject
    {
        public string TotalCount;
        public List<ProductStatistics> Data { get; set; }
    }

    public class ProductStatistics : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        //Code          - GenericClassObject
        //Description   - GenericClassObject

        public BranchStatistics P_HQ_BranchStatistics = new BranchStatistics();
        public ProductType Type = new ProductType();
        public Product_ECommerceSettings ECommerceSettings = new Product_ECommerceSettings();

        public ProductStatistics GetProductStatistics(long ID)
        {
            string apirequest = "ProductStatistics/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            ProductStatistics ProductStatistics = JsonConvert.DeserializeObject<ProductStatistics>(restResponse.Content);

            return ProductStatistics;
        }
    }    
}
