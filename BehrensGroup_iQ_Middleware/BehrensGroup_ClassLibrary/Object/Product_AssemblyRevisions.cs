/*
 * Author:      Ryan Curran
 * Date:        December 2019
 * Description: Class for Object Product.Assemblies
 *              Methods for Retreiving, Updating & Adding Product Assemblies via REST API
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

    public class Product_AssemblyRevision : GenericClassID
    {
        public string Number { get; set; }
        public string Description { get; set; }
        public string DefaultBuildType { get; set; }
        public string RevisionState { get; set; }
        public string FulfillmentType { get; set; }
        public string DescriptionOfService { get; set; }
        public Supplier DefaultSubContractSupplier = new Supplier();
        public Product SubContractProductCode = new Product();
        public string SupplierCostBase { get; set; }
        public List<Product_AssemblyRevision_Component> Components = new List<Product_AssemblyRevision_Component>();
    }

    public class Product_AssemblyRevision_Component : GenericClassID
    {
        public Product Product = new Product();
        public string Quantity { get; set; }
        public string StockAdjustmentRule { get; set; }
        public string TotalUsageValue { get; set; }
    }

    public class Product_Assembly : Product
    {
        public List<Product_AssemblyRevision> AssemblyRevisions = new List<Product_AssemblyRevision>();

        public void CreateProduct_AssemblyRevision()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "ProductAssemblyRevisions";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateProduct_AssemblyRevision_APIString(), ParameterType.RequestBody);

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

        public void UpdateProduct_AssemblyRevision()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "ProductAssemblies/" + ProductID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateProduct_AssemblyRevision_APIString(), ParameterType.RequestBody);

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

        public StringBuilder CreateProduct_AssemblyRevision_APIString()
        {
            StringBuilder APIString = new StringBuilder();

            APIString.Append("{");
            APIString.Append("\"AssemblyRevisions\":");
            APIString.Append("[{");
            int i = 1;
            foreach (Product_AssemblyRevision AssemblyRevision in Product_AssemblyRevisions)
            {
                if (i > 1) { APIString.Append(",{"); }
                APIString.Append(" \"Number\":                      \"" + AssemblyRevision.Number + "\"");
                APIString.Append(",\"Description\":                 \"" + AssemblyRevision.Description + "\"");
                APIString.Append(",\"DefaultBuildType\":            \"" + AssemblyRevision.DefaultBuildType + "\"");
                APIString.Append(",\"RevisionState\":               \"" + AssemblyRevision.RevisionState + "\"");
                APIString.Append(",\"FulfillmentType\":             \"" + AssemblyRevision.FulfillmentType + "\"");
                APIString.Append(",\"DefaultSubContractSupplier\":  \"" + AssemblyRevision.DefaultSubContractSupplier.Code + "\"");
                APIString.Append(",\"SubContractProductCode\":      \"" + AssemblyRevision.SubContractProductCode.ID + "\"");
                APIString.Append(",\"SupplierCostBase\":            \"" + AssemblyRevision.SupplierCostBase + "\"");
                APIString.Append(",\"Components\":");
                APIString.Append("[{");

                int j = 1;
                foreach (Product_AssemblyRevision_Component Component in AssemblyRevision.Components)
                {
                    if (j > 1) { APIString.Append(",{"); }
                    APIString.Append(" \"Product\":                      \"" + Component.Product.ID + "\"");
                    APIString.Append(",\"Quantity\":                     \"" + Component.Quantity + "\"");
                    APIString.Append(",\"StockAdjustmentRule\":          \"" + Component.StockAdjustmentRule + "\"");
                    APIString.Append(",\"TotalUsageValue\":              \"" + Component.TotalUsageValue + "\"");
                    APIString.Append("}");
                    j++;
                }
                APIString.Append("]");
                APIString.Append("}");
                i++;
            }
            APIString.Append("]");
            APIString.Append("}");
            return APIString;
        }
    }
}
