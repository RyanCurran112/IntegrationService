/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Model for Object Product
 *              
 */

using System;
using System.Collections.Generic;
using System.Text;

using Behrens_iQ_ClassLibrary.BaseClasses;

namespace Behrens_iQ_ClassLibrary.Models.Object
{
    public class Product : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        //Code          - GenericClassObject
        //Description   - GenericClassObject

        public int Type { get; set; }
        public string StrippedCode { get; set; }
        public long CategoryID { get; set; }
        public ProductCategory Category = new ProductCategory();
    }
}
