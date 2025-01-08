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
    public class CountryJSONObject
    {
        public string TotalCount;
        public List<Country> Data { get; set; }
    }

    public class Country : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        //Code          - GenericClassObject
        //Description   - GenericClassObject

        public static Country GetCountry(long ID)
        {
            string apirequest = "Countries/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            Country Country = JsonConvert.DeserializeObject<Country>(restResponse.Content);

            return Country;
        }

        public static Country GetCountryCode(string htmlparameter)
        {
            try
            {
                string apirequest = "Countries?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                CountryJSONObject Countries = JsonConvert.DeserializeObject<CountryJSONObject>(restResponse.Content);

                List<Country> listCountry = new List<Country>();
                foreach (Country country in Countries.Data)
                {
                    listCountry.Add(country);
                }
                return listCountry[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static List<Country> GetCountries(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "Countries?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                CountryJSONObject Countries = JsonConvert.DeserializeObject<CountryJSONObject>(restResponse.Content);

                List<Country> listCountry = new List<Country>();
                foreach (Country country in Countries.Data)
                {
                    listCountry.Add(country);
                }
                return listCountry;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Country Country = new Country { ID = 0 };
                List<Country> listCountry = new List<Country> { Country };
                return listCountry;
            }
        }

    }
}
