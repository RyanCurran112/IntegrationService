/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Data Access Layer for Object Delivery Instructions (Within Sales Orders)
 *              Methods for Retreiving, Updating, Adding Delivery Instructions via REST API
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
    class DAL_DeliveryInstructionsJSONObject
    {
        public string TotalCount;
        public List<DeliveryInstructions> Data { get; set; }
    }

    class DAL_DeliveryInstructions
    {
        public DeliveryInstructions GetCashCustomer(long ID)
        {
            string apirequest = "DeliveryInstructions/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            DeliveryInstructions deliveryInstructions = JsonSerializer.Deserialize<DeliveryInstructions>(restResponse.Content);

            return deliveryInstructions;
        }

        public List<DeliveryInstructions> GetDeliveryInstructions(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "DeliveryInstructions?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_DeliveryInstructionsJSONObject DeliveryInstructions = JsonSerializer.Deserialize<DAL_DeliveryInstructionsJSONObject>(restResponse.Content);

                List<DeliveryInstructions> listDeliveryInstructions = new List<DeliveryInstructions>();
                foreach (DeliveryInstructions deliveryInstructions in DeliveryInstructions.Data)
                {
                    listDeliveryInstructions.Add(deliveryInstructions);
                }

                return listDeliveryInstructions;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                DeliveryInstructions deliveryInstructions = new DeliveryInstructions();
                List<DeliveryInstructions> listDeliveryInstructions = new List<DeliveryInstructions>() { deliveryInstructions };

                return listDeliveryInstructions;
            }
        }

        public void CreateDeliveryInstructions(DeliveryInstructions deliveryInstructions)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.POST;
            string apirequest = "CashCustomers";

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateDeliveryInstructions_APIString(deliveryInstructions), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    deliveryInstructions.ID = RestItems.id;
                }
            }
        }

        public void UpdateCashCustomer(DeliveryInstructions deliveryInstructions)
        {
            var client = new RestClient("https://api.behrens.co.uk:1026/");
            Method method = Method.PUT;

            string apirequest = "CashCustomers/" + deliveryInstructions.ID;

            var request = new RestRequest(apirequest, method);
            request.AddHeader("Authorization", "Bearer OTg3ZGZlY2MtZDRhZC00OTAwLThiMD");
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("undefined", CreateDeliveryInstructions_APIString(deliveryInstructions), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            RESTReponse RestResponse = JsonSerializer.Deserialize<RESTReponse>(response.Content);

            if (RestResponse.items.Count > 0)
            {
                foreach (RestItemsReponse RestItems in RestResponse.items)
                {
                    deliveryInstructions.ID = RestItems.id;
                }
            }
        }

        public StringBuilder CreateDeliveryInstructions_APIString(DeliveryInstructions deliveryInstructions)
        {
            StringBuilder APIString = new StringBuilder();

            return APIString;
        }
    }
}
