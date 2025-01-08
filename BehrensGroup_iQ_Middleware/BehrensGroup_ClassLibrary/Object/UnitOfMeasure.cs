/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Unit Of Measure
 *              Methods for Retreiving Units Of Measure via REST API
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
    public class UnitOfMeasureJSONObject
    {
        public string TotalCount;
        public List<UnitOfMeasure> Data { get; set; }
    }
    public class UnitOfMeasure : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        //Code          - GenericClassObject
        //Description   - GenericClassObject

        public const long SellingUnits_Each_ID = 21539260997245;            //Each
        public const long SellingUnits_Box_ID = 21539261002189;             //Box
        public const long SellingUnits_Pack_ID = 21539261050966;            //Pack


        public UnitOfMeasure GetUnitOfMeasure(long ID)
        {
            string apirequest = "UnitsOfMeasure/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            UnitOfMeasure UnitOfMeasure = JsonConvert.DeserializeObject<UnitOfMeasure>(restResponse.Content);

            return UnitOfMeasure;
        }

        public UnitOfMeasure GetUnitOfMeasureCode(string htmlparameter)
        {
            try
            {
                string apirequest = "UnitsOfMeasure?Description[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                UnitOfMeasureJSONObject UnitsOfMeasure = JsonConvert.DeserializeObject<UnitOfMeasureJSONObject>(restResponse.Content);

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
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                UnitOfMeasureJSONObject UnitsOfMeasure = JsonConvert.DeserializeObject<UnitOfMeasureJSONObject>(restResponse.Content);

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
