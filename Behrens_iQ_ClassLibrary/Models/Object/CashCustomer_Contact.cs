/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Model for Object Cash Customer Contact
 *              
 */

using System;
using System.Collections.Generic;
using System.Text;

using Behrens_iQ_ClassLibrary.BaseClasses;

namespace Behrens_iQ_ClassLibrary.Models.Object
{
    public class CashCustomer_Contact : GenericClassID
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID

        public string UniqueID { get; set; }                //Unique ID is email address
        public _Salutation Salutation;
        public Customer Customer = new Customer();
        public Address Address = new Address();
        public enum _Salutation
        {
            Mr,
            Mrs,
            Dr,
            Ms,
            Miss
        }
    }
}
