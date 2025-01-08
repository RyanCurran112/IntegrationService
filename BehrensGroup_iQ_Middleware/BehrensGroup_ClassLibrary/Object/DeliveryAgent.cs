/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Delivery Agent
 *              Methods for Retreiving Delivery Agents via REST API
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
    public class DeliveryAgentJSONObject
    {
        public string TotalCount;
        public List<DeliveryAgent> Data { get; set; }
    }

    public class DeliveryAgent : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        //Code          - GenericClassObject
        //Description   - GenericClassObject
        
        public const long DeliveryAgent_CC_ID = 4771721956475;          //Customer Collection
        public const long DeliveryAgent_DPD_ID = 4771708727417;         //DPD
        public const long DeliveryAgent_GFS_ID = 4771708917264;         //GFS
        public const long DeliveryAgent_HAB_ID = 4771712464565;         //H&B
        public const long DeliveryAgent_HER_ID = 4771713422914;         //Hermes
        public const long DeliveryAgent_JET_ID = 4771708917258;         //JetSure
        public const long DeliveryAgent_RM_ID = 4771720044813;          //Royal Mail
        public const long DeliveryAgent_TBC_ID = 4771718439780;         //TBC
        public const long DeliveryAgent_TRA_ID = 4771708917261;         //Transbridge
        public const long DeliveryAgent_TUF_ID = 4771708917255;         //Tuffnells
        public const long DeliveryAgent_UKM_ID = 4771712958794;         //UK Mail
        public const long DeliveryAgent_UPS_ID = 4771708680942;         //UPS
        
        public static DeliveryAgent GetDeliveryAgent(long ID)
        {
            string apirequest = "DeliveryAgents/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            DeliveryAgent DeliveryAgent = JsonConvert.DeserializeObject<DeliveryAgent>(restResponse.Content);

            return DeliveryAgent;
        }

        public static DeliveryAgent GetDeliveryAgentCode(string htmlparameter)
        {
            try
            {
                string apirequest = "DeliveryAgents?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                DeliveryAgentJSONObject DeliveryAgents = JsonConvert.DeserializeObject<DeliveryAgentJSONObject > (restResponse.Content);

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

        public static List<DeliveryAgent> GetDeliveryAgents(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "DeliveryAgents?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                DeliveryAgentJSONObject DeliveryAgents = JsonConvert.DeserializeObject<DeliveryAgentJSONObject>(restResponse.Content);

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
                DeliveryAgent DeliveryAgent = new DeliveryAgent() { ID = 0 };
                List<DeliveryAgent> listDeliveryAgent = new List<DeliveryAgent>() { DeliveryAgent };
                return listDeliveryAgent;
            }
        }
    }
}
