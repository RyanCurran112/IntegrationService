/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Sales Order Source
 *              Methods for Retreiving Sales Order Sources via REST API
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
    public class SalesOrderSourceJSONObject
    {
        public string TotalCount;
        public List<SalesOrderSource> Data { get; set; }
    }
    public class SalesOrderSource : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        //Code          - GenericClassObject
        //Description   - GenericClassObject

        public const long OrderSource_CPT_ID = 9264244609311;           //CorporateTrends.co.uk
        public const long OrderSource_DAD_ID = 9264263621266;           //DipAndDoze.com
        public const long OrderSource_LBY_ID = 9264244609313;           //LaBeeby.co.uk
        public const long OrderSource_NEDI_ID = 9264278731204;         //NeTiX
        public const long OrderSource_SEDI_ID = 9264289251828;          //Scotts
        public const long OrderSource_FEDI_ID = 9264299470608;          //Foodbuy

        public static SalesOrderSource GetSalesOrderSource(long ID)
        {
            string apirequest = "SalesOrderSources/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            SalesOrderSource SalesOrderSource = JsonConvert.DeserializeObject<SalesOrderSource>(restResponse.Content);

            return SalesOrderSource;
        }

        public static SalesOrderSource GetSalesOrderSourceCode(string htmlparameter)
        {
            try
            {
                string apirequest = "SalesOrderSources?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                SalesOrderSourceJSONObject SalesOrderSources = JsonConvert.DeserializeObject<SalesOrderSourceJSONObject>(restResponse.Content);

                List<SalesOrderSource> listSalesOrderSource = new List<SalesOrderSource>();
                foreach (SalesOrderSource salesOrderSource in SalesOrderSources.Data)
                {
                    listSalesOrderSource.Add(salesOrderSource);
                }
                return listSalesOrderSource[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static List<SalesOrderSource> GetSalesOrderSources(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "SalesOrderSources?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                SalesOrderSourceJSONObject SalesOrderSources = JsonConvert.DeserializeObject<SalesOrderSourceJSONObject>(restResponse.Content);

                List<SalesOrderSource> listSalesOrderSource = new List<SalesOrderSource>();
                foreach (SalesOrderSource salesOrderSource in SalesOrderSources.Data)
                {
                    listSalesOrderSource.Add(salesOrderSource);
                }
                return listSalesOrderSource;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                SalesOrderSource SalesOrderSource = new SalesOrderSource();
                List<SalesOrderSource> listSalesOrderSource = new List<SalesOrderSource>() { SalesOrderSource };
                return listSalesOrderSource;
            }
        }
    }
}
