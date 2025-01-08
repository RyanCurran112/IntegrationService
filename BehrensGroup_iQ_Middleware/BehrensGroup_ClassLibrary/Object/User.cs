using System;
using System.Collections.Generic;
using System.Text;

namespace BehrensGroup_ClassLibrary.Object
{
    public class User : BehrensGroup_ClassLibrary.BaseClasses.GenericClassObject
    {
        //ID            - GenericClassID
        //CreatedOn     - GenericClassID
        //CreatedBy     - GenericClassID
        //UpdatedOn     - GenericClassID
        //UpdatedBy     - GenericClassID
        //Code          - GenericClassObject
        //Description   - GenericClassObject

        public long BSID { get; set; }
        public long IQID { get; set; }

        public string BSUsername { get; set; }
        public string BSPassword { get; set; }
        public string DomainUsername { get; set; }

        public string CurrentView { get; set; }
        public DateTime LastLogin { get; set; }
        public int UStatus { get; set; }
        public string Access { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string HomePage { get; set; }
        public string JobTitle { get; set; }
        public string Employee { get; set; }
        public string SignatureImage { get; set; }

        public string PrimaryDivision { get; set; }
        public string SecondaryDivision { get; set; }


        public string FirstName()
        {
            string[] values = Name.Split(' ');
            return values[0];
        }

        public string Surname()
        {
            string[] values = Name.Split(' ');
            return values[1];
        }

        public void GetCurrentUser()
        {

        }

        public void GetUser()
        {

        }
    }
}
