/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Sales Order Type
 *              Methods for Retreiving Sales Order Types via REST API
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
    class DAL_SalesOrderTypeJSONObject
    {
        public string TotalCount;
        public List<SalesOrderType> Data { get; set; }
    }

    class DAL_SalesOrderType
    {
        public SalesOrderType GetSalesOrderType(long ID)
        {
            string apirequest = "SalesOrderTypes/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            SalesOrderType SalesOrderType = JsonSerializer.Deserialize<SalesOrderType>(restResponse.Content);

            return SalesOrderType;
        }

        public SalesOrderType GetSalesOrderTypeCode(string htmlparameter)
        {
            try
            {
                string apirequest = "SalesOrderTypes?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_SalesOrderTypeJSONObject SalesOrderTypes = JsonSerializer.Deserialize<DAL_SalesOrderTypeJSONObject>(restResponse.Content);

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

        public List<SalesOrderType> GetSalesOrderTypes(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "SalesOrderTypes?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_SalesOrderTypeJSONObject SalesOrderTypes = JsonSerializer.Deserialize<DAL_SalesOrderTypeJSONObject>(restResponse.Content);

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
