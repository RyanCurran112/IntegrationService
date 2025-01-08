/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Unit of Measure
 *              Methods for Retreiving Units of Measure via REST API
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
    class DAL_UnitOfMeasureJSONObject
    {
        public string TotalCount;
        public List<UnitOfMeasure> Data { get; set; }
    }
    class DAL_UnitOfMeasure
    {
        public UnitOfMeasure GetUnitOfMeasure(long ID)
        {
            string apirequest = "UnitsOfMeasure/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
            UnitOfMeasure UnitOfMeasure = JsonSerializer.Deserialize<UnitOfMeasure>(restResponse.Content);

            return UnitOfMeasure;
        }

        public UnitOfMeasure GetUnitOfMeasureCode(string htmlparameter)
        {
            try
            {
                string apirequest = "UnitsOfMeasure?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_UnitOfMeasureJSONObject UnitsOfMeasure = JsonSerializer.Deserialize<DAL_UnitOfMeasureJSONObject>(restResponse.Content);

                List<UnitOfMeasure> listUnitOfMeasure = new List<UnitOfMeasure>();
                foreach (UnitOfMeasure unitOfMeasure in UnitsOfMeasure.Data)
                {
                    listUnitOfMeasure.Add(unitOfMeasure);
                }
                return listUnitOfMeasure[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<UnitOfMeasure> GetUnitsOfMeasure(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "UnitsOfMeasure?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = BEH_RestClient.RestClientFunction(apirequest, method);
                DAL_UnitOfMeasureJSONObject UnitsOfMeasure = JsonSerializer.Deserialize<DAL_UnitOfMeasureJSONObject>(restResponse.Content);

                List<UnitOfMeasure> listUnitOfMeasure = new List<UnitOfMeasure>();
                foreach (UnitOfMeasure unitOfMeasure in UnitsOfMeasure.Data)
                {
                    listUnitOfMeasure.Add(unitOfMeasure);
                }
                return listUnitOfMeasure;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                UnitOfMeasure UnitOfMeasure = new UnitOfMeasure();
                List<UnitOfMeasure> listUnitOfMeasure = new List<UnitOfMeasure>() { UnitOfMeasure };
                return listUnitOfMeasure;
            }
        }
    }
}
