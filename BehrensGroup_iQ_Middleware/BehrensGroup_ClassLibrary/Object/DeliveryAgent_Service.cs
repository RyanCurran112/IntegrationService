/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Delivery Agent Service
 *              Methods for Retreiving Delivery Agent Services via REST API
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
    public class DeliveryAgent_ServiceJSONObject
    {
        public string TotalCount;
        public List<DeliveryAgent_Service> Data { get; set; }
    }
    public class DeliveryAgent_Service : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        //Code          - GenericClassObject
        //Description   - GenericClassObject

        public const long DeliveryAgentService_CC_ID = 4776027685226;           //Customer Collection
        public const long DeliveryAgentService_DPD_ID = 4776024561139;          //DPD Parcel Next Day

        public const long DeliveryAgentService_GFS_INT_ID = 4776015926269;      //GFS International
        public const long DeliveryAgentService_GFS_ND_ID = 4776013764837;       //GFS Next Day

        public const long DeliveryAgentService_HAB_ID = 4776009049105;          //H&B Next Day

        public const long DeliveryAgentService_HER_ID = 4776015185420;          //Hermes 2 Day

        public const long DeliveryAgentService_JET_ID = 4776003884553;          //Jetsure Next Day

        public const long DeliveryAgentService_RM_ID = 4776022563850;           //Royal Mail - Tracked 48
        public const long DeliveryAgentService_RM1_ID = 4776015012107;          //Royal Mail - 1st Class

        public const long DeliveryAgentService_TBC_ID = 4776013407075;          //TBC

        public const long DeliveryAgentService_TRA_ID = 4776003884556;          //Transbridge Standard 2 Day

        public const long DeliveryAgentService_TUF_P1_ID = 4776013373969;       //Tuffnells Next Day

        public const long DeliveryAgentService_UKM_ID = 4776015186071;          //UKMail Next Day
      
        public const long DeliveryAgentService_UPS_ID = 4776003648236;          //UPS Standard Next Day

        public static DeliveryAgent_Service GetDeliveryAgent_Service(long ID)
        {
            string apirequest = "DeliveryAgentServices/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            DeliveryAgent_Service DeliveryAgent_Service = JsonConvert.DeserializeObject<DeliveryAgent_Service>(restResponse.Content);

            return DeliveryAgent_Service;
        }

        public static List<DeliveryAgent_Service> GetDeliveryAgents_Services(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "DeliveryAgentServices?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                DeliveryAgent_ServiceJSONObject DeliveryAgent_Services = JsonConvert.DeserializeObject<DeliveryAgent_ServiceJSONObject>(restResponse.Content);

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
                DeliveryAgent_Service DeliveryAgent_Service = new DeliveryAgent_Service() { ID = 0 };
                List<DeliveryAgent_Service> listDeliveryAgent_Service = new List<DeliveryAgent_Service>() { DeliveryAgent_Service };
                return listDeliveryAgent_Service;
            }
        }
    }
}
