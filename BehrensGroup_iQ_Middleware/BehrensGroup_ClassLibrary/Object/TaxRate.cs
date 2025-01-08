/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Tax Rate
 *              Methods for Retreiving Tax Rates via REST API
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
    public class TaxRateJSONObject
    {
        public string TotalCount;
        public List<TaxRate> Data { get; set; }
    }

    public class TaxRate : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        //Code          - GenericClassObject
        //Description   - GenericClassObject

        public const long TaxRate_GB01_ID = 4325032074116;          //GB01 20% Tax
        public const long TaxRate_GB04_ID = 4325036178587;          //GB04 Exempt 0% Tax
        public const long TaxRate_GB05_ID = 4325032098820;          //GB05 Export

        public TaxRate GetTaxRate(long ID)
        {
            string apirequest = "TaxRates/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            TaxRate TaxRate = JsonConvert.DeserializeObject<TaxRate>(restResponse.Content);

            return TaxRate;
        }

        public TaxRate GetTaxRateCode(string htmlparameter)
        {
            try
            {
                string apirequest = "TaxRates?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                TaxRateJSONObject TaxRates = JsonConvert.DeserializeObject<TaxRateJSONObject>(restResponse.Content);

                List<TaxRate> listTaxRate = new List<TaxRate>();
                foreach (TaxRate taxRate in TaxRates.Data)
                {
                    listTaxRate.Add(taxRate);
                }
                return listTaxRate[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<TaxRate> GetTaxRates(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "TaxRates?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                TaxRateJSONObject TaxRates = JsonConvert.DeserializeObject<TaxRateJSONObject>(restResponse.Content);

                List<TaxRate> listTaxRate = new List<TaxRate>();
                foreach (TaxRate taxRate in TaxRates.Data)
                {
                    listTaxRate.Add(taxRate);
                }
                return listTaxRate;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                TaxRate TaxRate = new TaxRate();
                List<TaxRate> listTaxRate = new List<TaxRate>() { TaxRate };
                return listTaxRate;
            }
        }

    }
}
