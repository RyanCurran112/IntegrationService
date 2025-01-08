/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Model for Object Customer
 *              
 */

using System;
using System.Collections.Generic;
using System.Text;

using Behrens_iQ_ClassLibrary.BaseClasses;

namespace Behrens_iQ_ClassLibrary.Models.Object
{
    public class Customer : GenericClassID
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID

        public string Code { get; set; }
        public string Name { get; set; }
        public string D_TradingNamePrefix { get; set; }
        public string TradingName { get; set; }
        public DateTime AccountOpenedOn { get; set; }

        //Contact Info
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string EmailAddress { get; set; }
        public string Homepage { get; set; }
        public string D_Instagram { get; set; }
        public string D_Facebook { get; set; }
        public string D_Twitter { get; set; }


        //Location
        public Address Address = new Address();
        public Region Region = new Region();
    }
}
