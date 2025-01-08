using System;
using System.Collections.Generic;
using System.Text;

namespace Behrens_iQ_ClassLibrary.BaseClasses
{
    class RESTReponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public string queueEntryId { get; set; }

        public List<RestItemsReponse> items = new List<RestItemsReponse>();
    }
    class RestItemsReponse
    {
        public long id { get; set; }
        public string typeName { get; set; }
        public string info { get; set; }
    }
}
