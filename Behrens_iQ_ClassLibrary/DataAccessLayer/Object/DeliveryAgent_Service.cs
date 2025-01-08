/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Data Access Layer for Object Delivery Agent Service
 *              Methods for Retreiving Delivery Agent Service via REST API
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
    class DAL_DeliveryAgent_ServiceJSONObject
    {
        public string TotalCount;
        public List<DeliveryAgent> Data { get; set; }
    }
    class DAL_DeliveryAgent_Service
    {
        public DeliveryAgent_Service GetDeliveryAgent_Service(long ID)
        {
            string apirequest = "DeliveryAgentServices/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            DeliveryAgent_Service DeliveryAgent_Service = JsonSerializer.Deserialize<DeliveryAgent_Service>(restResponse.Content);

            return DeliveryAgent_Service;
        }

        public List<DeliveryAgent_Service> GetDeliveryAgents_Services(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "DeliveryAgentServices?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_DeliveryAgent_ServiceJSONObject DeliveryAgent_Services = JsonSerializer.Deserialize<DAL_DeliveryAgent_ServiceJSONObject>(restResponse.Content);

                List<DeliveryAgent_Service> listDeliveryAgent_Service = new List<DeliveryAgent_Service>();
                foreach (DeliveryAgent_Service deliveryAgent_Service in DeliveryAgent_Services.Data)
                {
                    listDeliveryAgent_Service.Add(deliveryAgent_Service);
                }
                return listDeliveryAgent_Service;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                DeliveryAgent_Service DeliveryAgent_Service = new DeliveryAgent_Service();
                DeliveryAgent_Service.ID = 0;
                List<DeliveryAgent_Service> listDeliveryAgent_Service = new List<DeliveryAgent_Service>();

                listDeliveryAgent_Service.Add(DeliveryAgent_Service);
                return listDeliveryAgent_Service;
            }
        }
    }
}
