/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for Object Product
 *              Methods for Retreiving Branch Statistics via REST API
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
    public class BranchStatisticsJSONObject
    {
        public string TotalCount;
        public List<BranchStatistics> Data { get; set; }
    }

    public class BranchStatistics : GenericClassID
    {
        //ID            - GenericClassID
        public decimal UnallocatedStockLevel { get; set; }
        public decimal CALC_WebStockLevel { get; set; }
    }
}
