using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehrensGroup_MiddlewareIQ.Inbound.Couriers
{
    class Couriers
    {
        public static void CouriersMain()
        {
            
            RoyalMail.RoyalMailCourierMain();
            DeliveringExactly.DeliveringExactlyCourierMain();

            Parcelforce.ParcelforceCourierMain();

            GFS.GFSCourierMain();
            
            DPD.DPDCourierMain();
            UPS.UPSCourierMain();

            Console.WriteLine("Inbound - Courier Information Complete");
        }
    }
}
