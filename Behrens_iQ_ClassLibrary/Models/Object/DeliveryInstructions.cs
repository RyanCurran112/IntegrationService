/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Model for Object Delivery Instructions
 *              
 */

using System;
using System.Collections.Generic;
using System.Text;

using Behrens_iQ_ClassLibrary.BaseClasses;

namespace Behrens_iQ_ClassLibrary.Models.Object
{
    public class DeliveryInstructions : GenericClassID
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID

        public string Text;
    }
}
