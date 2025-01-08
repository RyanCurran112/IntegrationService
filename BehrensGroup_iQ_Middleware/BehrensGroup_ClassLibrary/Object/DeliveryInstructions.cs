using BehrensGroup_ClassLibrary.BaseClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BehrensGroup_ClassLibrary.Object
{
    public class DeliveryInstructions : GenericClassObject
    {
        private string text;
        public string Text 
        {
            get { return text; }
            set { if (value != null) { text = value.Replace(",", "."); } } 
        }
    }
}
