using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Behrens_iQ_ClassLibrary.BaseClasses
{
    public class GenericClassID : System.Object
    {
        [Key]
        public long ID { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
    public class GenericClassObject : GenericClassID
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID

        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class GenericClassTransactionNumber : GenericClassID
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        public string Number { get; set; }
        public string CreatedDate { get; set; }
    }

    public class GenericClassTransaction : GenericClassTransactionNumber
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        //Number        - GenericClassTransactionNumber
        //CreatedDate   - GenericClassTransactionNumber
    }

    public class GenericClassTransactionLine : GenericClassTransactionNumber
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID
        //Number        - GenericClassTransactionNumber
        //CreatedDate   - GenericClassTransactionNumber
        public int LineNo { get; set; }
    }
    public class GenericClassWorkflowStatus : GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //UpdatedOn     - GenericClassID

        //Code          - GenericClassObject
        //Description   - GenericClassObject
    }
}
