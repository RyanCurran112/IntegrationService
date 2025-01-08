/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Sales Order Promotion
 *              Methods for Retreiving Sales Order Promotions via REST API
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
    class DAL_SalesPromotionJSONObject
    {
        public string TotalCount;
        public List<SalesPromotion> Data { get; set; }
    }
    class DAL_SalesPromotion
    {
        public SalesPromotion GetSalesPromotion(long ID)
        {
            string apirequest = "SalesPromotions/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            SalesPromotion SalesPromotion = JsonSerializer.Deserialize<SalesPromotion>(restResponse.Content);

            return SalesPromotion;
        }

        public List<SalesPromotion> GetSalesPromotions(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "SalesPromotions?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_SalesPromotionJSONObject SalesPromotions = JsonSerializer.Deserialize<DAL_SalesPromotionJSONObject>(restResponse.Content);

                List<SalesPromotion> listSalesPromotion = new List<SalesPromotion>();
                foreach (SalesPromotion salesPromotion in SalesPromotions.Data)
                {
                    listSalesPromotion.Add(salesPromotion);
                }
                return listSalesPromotion;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                SalesPromotion SalesPromotion = new SalesPromotion();
                SalesPromotion.ID = 0;
                List<SalesPromotion> listSalesPromotion = new List<SalesPromotion>();

                listSalesPromotion.Add(SalesPromotion);
                return listSalesPromotion;
            }
        }

        public void CreateSalesPromotion(SalesPromotion salesPromotion)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "SalesPromotions";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateSalesPromotion_APIString(salesPromotion), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    salesPromotion.ID = RestItems.id;
                }
            }
        }

        public void UpdateSalesPromotion(SalesPromotion salesPromotion)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "SalesPromotions/" + salesPromotion.ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateSalesPromotion_APIString(salesPromotion), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    salesPromotion.ID = RestItems.id;
                }
            }
        }

        public StringBuilder CreateSalesPromotion_APIString(SalesPromotion salesPromotion)
        {
            StringBuilder APIString = new StringBuilder();

            APIString.Append("{");
            APIString.Append("\"Code\":\"" + salesPromotion.Code + "\"");
            APIString.Append(",\"Description\":\"" + salesPromotion.Description + "\"");
            APIString.Append("}");

            return APIString;
        }
    }
}
