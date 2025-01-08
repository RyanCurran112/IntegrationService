/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Currency
 *              Methods for Retreiving Currencies via REST API
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
    public class CurrencyJSONObject
    {
        public string TotalCount;
        public List<Currency> Data { get; set; }
    }

    public class Currency : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        //Code          - GenericClassObject
        //Description   - GenericClassObject

        public const long Currency_GBP_ID = 4346506903774;

        public decimal ExchangeRate { get; set; }

        public static Currency GetCurrency(long ID)
        {
            string apirequest = "Currencies/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            Currency Currency = JsonConvert.DeserializeObject<Currency>(restResponse.Content);

            return Currency;
        }

        public static Currency GetCurrencyCode(string htmlparameter)
        {
            try
            {
                string apirequest = "Currencies?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                CurrencyJSONObject Currencies = JsonConvert.DeserializeObject<CurrencyJSONObject>(restResponse.Content);

                List<Currency> listCurrency = new List<Currency>();
                foreach (Currency currency in Currencies.Data)
                {
                    listCurrency.Add(currency);
                }
                return listCurrency[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static List<Currency> GetCurrencies(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "Currencies?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                CurrencyJSONObject Currencies = JsonConvert.DeserializeObject<CurrencyJSONObject>(restResponse.Content);

                List<Currency> listCurrency = new List<Currency>();
                foreach (Currency currency in Currencies.Data)
                {
                    listCurrency.Add(currency);
                }
                return listCurrency;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Currency Currency = new Currency() { ID = 0 };
                List<Currency> listCurrency = new List<Currency> { Currency };
                return listCurrency;
            }
        }
    }
}
