/*
 * Author:      Ryan Curran
 * Date:        December 2019
 * Description: Class for Object Supplier
 *              Methods for Retreiving Suppliers via REST API
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
    public class Supplier : GenericClassObject
    {
        public string Name { get; set; }
    }
}
