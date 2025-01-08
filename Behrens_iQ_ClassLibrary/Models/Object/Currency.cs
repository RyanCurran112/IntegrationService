/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Model for Object Currency
 *              
 */

using System;
using System.Collections.Generic;
using System.Text;

using Behrens_iQ_ClassLibrary.BaseClasses;

namespace Behrens_iQ_ClassLibrary.Models.Object
{
    public class Currency : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID

        //Code          - GenericClassObject
        //Description   - GenericClassObject

        public decimal ExchangeRate { get; set; }
    }
}
