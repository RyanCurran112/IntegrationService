/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Data Access Layer for Object Delivery Agent
 *              Methods for Retreiving Delivery Agent via REST API
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
    class DeliveryAgentJSONObject
    {
        public string TotalCount;
        public List<DeliveryAgent> Data { get; set; }
    }

    class DAL_DeliveryAgent
    {
        public DeliveryAgent GetDeliveryAgent(long ID)
        {
            string apirequest = "DeliveryAgents/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            DeliveryAgent DeliveryAgent = JsonSerializer.Deserialize<DeliveryAgent>(restResponse.Content);

            return DeliveryAgent;
        }

        public DeliveryAgent GetDeliveryAgentCode(string htmlparameter)
        {
            try
            {
                string apirequest = "DeliveryAgents?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DeliveryAgentJSONObject DeliveryAgents = JsonSerializer.Deserialize<DeliveryAgentJSONObject>(restResponse.Content);

                List<DeliveryAgent> listDeliveryAgent = new List<DeliveryAgent>();
                foreach (DeliveryAgent deliveryAgent in DeliveryAgents.Data)
                {
                    listDeliveryAgent.Add(deliveryAgent);
                }
                return listDeliveryAgent[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<DeliveryAgent> GetDeliveryAgents(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "DeliveryAgents?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DeliveryAgentJSONObject DeliveryAgents = JsonSerializer.Deserialize<DeliveryAgentJSONObject>(restResponse.Content);

                List<DeliveryAgent> listDeliveryAgent = new List<DeliveryAgent>();
                foreach (DeliveryAgent deliveryAgent in DeliveryAgents.Data)
                {
                    listDeliveryAgent.Add(deliveryAgent);
                }
                return listDeliveryAgent;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                DeliveryAgent DeliveryAgent = new DeliveryAgent();
                List<DeliveryAgent> listDeliveryAgent = new List<DeliveryAgent>() { DeliveryAgent };
                return listDeliveryAgent;
            }
        }
    }
}
