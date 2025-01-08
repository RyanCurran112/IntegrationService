/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Sales Order Type
 *              Methods for Retreiving Sales Order Types via REST API
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
    public class SalesOrderTypeJSONObject
    {
        public string TotalCount;
        public List<SalesOrderType> Data { get; set; }
    }

    public class SalesOrderType : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        //Code          - GenericClassObject
        //Description   - GenericClassObject

        public const long OrderType_WebCashOrders_ID = 9247079131646;           //Web Cash Order
        public const long OrderType_WebOrders_ID = 9247064678828;               //Web Order
        public const long OrderType_EDI_ID = 9247064740079;                     //EDI Order
        public const long OrderType_LECTURERS_ID = 9247129398961;               //Lecturers Order
        public static SalesOrderType GetSalesOrderType(long ID)
        {
            string apirequest = "SalesOrderTypes/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            SalesOrderType SalesOrderType = JsonConvert.DeserializeObject<SalesOrderType>(restResponse.Content);

            return SalesOrderType;
        }

        public static SalesOrderType GetSalesOrderTypeCode(string htmlparameter)
        {
            try
            {
                string apirequest = "SalesOrderTypes?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                SalesOrderTypeJSONObject SalesOrderTypes = JsonConvert.DeserializeObject<SalesOrderTypeJSONObject>(restResponse.Content);

                List<SalesOrderType> listSalesOrderType = new List<SalesOrderType>();
                foreach (SalesOrderType salesOrderType in SalesOrderTypes.Data)
                {
                    listSalesOrderType.Add(salesOrderType);
                }
                return listSalesOrderType[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);               
                return null;
            }
        }

        public static List<SalesOrderType> GetSalesOrderTypes(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "SalesOrderTypes?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                SalesOrderTypeJSONObject SalesOrderTypes = JsonConvert.DeserializeObject<SalesOrderTypeJSONObject>(restResponse.Content);

                List<SalesOrderType> listSalesOrderType = new List<SalesOrderType>();
                foreach (SalesOrderType salesOrderType in SalesOrderTypes.Data)
                {
                    listSalesOrderType.Add(salesOrderType);
                }
                return listSalesOrderType;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return null;
            }
        }
    }
}
