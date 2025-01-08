/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Division
 *              Methods for Retreiving Divisions via REST API
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
    public class DivisionJSONObject
    {
        public string TotalCount;
        public List<Division> Data { get; set; }
    }

    public class Division : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID

        //Code          - GenericClassObject
        //Description   - GenericClassObject

        public const long Division_BEH_ID = 39232911374255618;              //Behrens
        public const long Division_BHT_ID = 39232911341392678;              //Home Textiles
        public const long Division_CPT_ID = 39232911341392717;              //CorporateTrends
        public const long Division_DAD_ID = 39232911349295554;              //DipAndDoze
        public const long Division_FPC_ID = 39232911341349037;              //Healthcare
        public const long Division_HHB_ID = 39232911341392693;              //Corporate & Workwear
        public const long Division_LBY_ID = 39232911341392714;              //LaBeeby
        public const long Division_NDV_ID = 39232911341349040;              //Healthcare Uniforms

        public static Division GetDivision(long ID)
        {
            string apirequest = "Divisions/" + ID;

            Method method = Method.GET;
            IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
            Division Division = JsonConvert.DeserializeObject<Division>(restResponse.Content);

            return Division;
        }

        public static Division GetDivisionCode(string htmlparameter)
        {
            try
            {
                string apirequest = "Divisions?Code[eq]=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                DivisionJSONObject Divisions = JsonConvert.DeserializeObject<DivisionJSONObject>(restResponse.Content);

                List<Division> listDivision = new List<Division>();
                foreach (Division division in Divisions.Data)
                {
                    listDivision.Add(division);
                }
                return listDivision[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static List<Division> GetDivisions(string htmloperator, string htmlattribute, string htmlparameter)
        {
            try
            {
                string apirequest = "Divisions?" + htmlattribute + htmloperator + "=" + htmlparameter;

                Method method = Method.GET;
                IRestResponse restResponse = RestClient2.RestClientFunction(apirequest, method);
                DivisionJSONObject Divisions = JsonConvert.DeserializeObject<DivisionJSONObject>(restResponse.Content);

                List<Division> listDivision = new List<Division>();
                foreach (Division division in Divisions.Data)
                {
                    listDivision.Add(division);
                }
                return listDivision;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Division Division = new Division();
                List<Division> listDivision = new List<Division>() { Division };
                return listDivision;
            }
        }
    }
}
