using System;
using System.Collections.Generic;
using System.Text;

namespace BehrensGroup_ClassLibrary.Object
{
    public class Product_ECommerceSettings
    {
        public bool PreventThisItemBeingSoldViaECommerce { get; set; }
        public Product_ECommerceSettings_ECommerceStatus ECommerceStatus = new Product_ECommerceSettings_ECommerceStatus();

    }
}
