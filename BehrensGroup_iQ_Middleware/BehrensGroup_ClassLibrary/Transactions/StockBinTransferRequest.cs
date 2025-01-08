/*
 * Author:      Ryan Curran
 * Date:        Jan 2021
 * Description: Class for Object StockBinTranferRequest
 *              Methods for Retreiving, Updating & Adding Stock Bin Transfer Request via REST API
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using Newtonsoft.Json;
using RestSharp;

using BehrensGroup_ClassLibrary.BaseClasses;
using BehrensGroup_ClassLibrary.Object;
using BehrensGroup_ClassLibrary.Functions;

namespace BehrensGroup_ClassLibrary.Transactions
{
    public class StockBinTransferRequestJSONObject
    {
        public string TotalCount;
        public List<StockBinTransferRequest> Data { get; set; }
    }

    public class StockBinTransferRequest : GenericClassTransaction
    {
        //ID                    - GenericClassID
        //Number                - GenericClassTransactionNumber
        //CreatedDate           - GenericClassTransactionNumber

        public string RequestedBy { get; set; }

        //Order Information
        private string date;
        public string Date
        {
            get { return date; }
            set { date = Convert.ToDateTime(value).ToString().Replace("/", "-").Replace(":", "-"); }
        }
        public string Status { get; set; }
        

        public List<StockBinTransferRequest_Line> StockBinTransferRequestLines = new List<StockBinTransferRequest_Line>();


        public StockBinTransferRequest GetStockBinTransferRequest(long ID)
        {
            string apirequest = "StockBinTransferRequests/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            StockBinTransferRequest StockBinTransferRequest = JsonConvert.DeserializeObject<StockBinTransferRequest>(restResponse.Content);

            return StockBinTransferRequest;
        }

        public List<StockBinTransferRequest> GetStockBinTransferRequests(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "StockBinTransferRequests?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                StockBinTransferRequestJSONObject StockBinTransferRequests = JsonConvert.DeserializeObject<StockBinTransferRequestJSONObject>(restResponse.Content);

                List<StockBinTransferRequest> listStockBinTransferRequests = new List<StockBinTransferRequest>();
                foreach (StockBinTransferRequest StockBinTransferRequest in StockBinTransferRequests.Data)
                {
                    listStockBinTransferRequests.Add(StockBinTransferRequest);
                }
                return listStockBinTransferRequests;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                StockBinTransferRequest StockBinTransferRequest = new StockBinTransferRequest();

                List<StockBinTransferRequest> listStockBinTransferRequest = new List<StockBinTransferRequest>
            {
                StockBinTransferRequest
            };
                return listStockBinTransferRequest;
            }
        }

        public void CreateStockBinTransferRequest()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "StockBinTransferRequests";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateStockBinTransferRequest_APIString(), ParameterType.RequestBody);

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

        public StringBuilder CreateStockBinTransferRequest_APIString()
        {
            StringBuilder APIString = new StringBuilder();


            APIString.Append("{");
            APIString.Append(" \"RequestedBy\":       \"" + RequestedBy + "\"");
            
            APIString.Append(",\"Items\": ");
            APIString.Append("[{");

            int i = 1;
            foreach (StockBinTransferRequest_Line line in StockBinTransferRequestLines)
            {
                if (i > 1) { APIString.Append(",{"); }

                APIString.Append(" \"Product\":                  \"" + line.Product.Code + "\"");

                if (line.ProductBatch.ID != 0) { APIString.Append(",\"ProductBatch\":        \"" + line.ProductBatch.ID + "\""); }
                if (line.FromStockBin.ID != 0) { APIString.Append(",\"FromStockBin\":          \"" + line.FromStockBin.ID + "\""); }
                if (line.ToStockBin.ID != 0) { APIString.Append(",\"ToStockBin\":              \"" + line.ToStockBin.ID + "\""); }

                APIString.Append(",\"Quantity\":                 \"" + line.Quantity + "\"");


                APIString.Append("}");
                i++;
            }

            APIString.Append("]");

            APIString.Append("}");

            return APIString;
        }
    }
}
