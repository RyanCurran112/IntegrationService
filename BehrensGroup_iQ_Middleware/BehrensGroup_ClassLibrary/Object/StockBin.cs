/*
 * Author:      Ryan Curran
 * Date:        January 2021
 * Description: Class for Object Stock Bin
 *              Methods for Retreiving Stock Bins via REST API
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
    public class StockBinJSONObject
    {
        public string TotalCount;
        public List<StockBin> Data { get; set; }
    }

    public class StockBin : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        //Code          - GenericClassObject
        //Description   - GenericClassObject

    }
}
