/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Sales Promotion
 *              Methods for Retreiving Sales Promotions via REST API
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
    public class SalesPromotionJSONObject
    {
        public string TotalCount;
        public List<SalesPromotion> Data { get; set; }
    }

    public class SalesPromotion : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID

        //Code          - GenericClassObject
        //Description   - GenericClassObject

        public SalesPromotion GetSalesPromotion(long ID)
        {
            string apirequest = "SalesPromotions/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            SalesPromotion SalesPromotion = JsonConvert.DeserializeObject<SalesPromotion>(restResponse.Content);

            return SalesPromotion;
        }

        public SalesPromotion GetSalesPromotionCode(string htmlparameter)
        {
            try
            {
                string apirequest = "SalesPromotions?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                SalesPromotionJSONObject SalesPromotions = JsonConvert.DeserializeObject<SalesPromotionJSONObject>(restResponse.Content);

                List<SalesPromotion> listSalesPromotion = new List<SalesPromotion>();
                foreach (SalesPromotion salesPromotion in SalesPromotions.Data)
                {
                    ID = salesPromotion.ID;
                    Code = salesPromotion.Code;

                    listSalesPromotion.Add(salesPromotion);
                }
                return listSalesPromotion[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<SalesPromotion> GetSalesPromotions(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "SalesPromotions?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                SalesPromotionJSONObject SalesPromotions = JsonConvert.DeserializeObject<SalesPromotionJSONObject>(restResponse.Content);

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
                List<SalesPromotion> listSalesPromotion = new List<SalesPromotion>() { SalesPromotion };
                return listSalesPromotion;
            }
        }

        public void CreateSalesPromotion()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "SalesPromotions";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateSalesPromotion_APIString(), ParameterType.RequestBody);

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

        public void UpdateSalesPromotion()
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "SalesPromotions/" + ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateSalesPromotion_APIString(), ParameterType.RequestBody);

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

        public StringBuilder CreateSalesPromotion_APIString()
        {
            StringBuilder APIString = new StringBuilder();

            APIString.Append("{");
            APIString.Append("\"Code\":\"" + Code + "\"");
            APIString.Append(",\"Description\":\"" + Description + "\"");
            APIString.Append("}");

            return APIString;
        }
    }
}
