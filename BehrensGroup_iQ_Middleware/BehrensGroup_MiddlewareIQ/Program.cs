using System;

namespace BehrensGroup_MiddlewareIQ
{
    class Program
    {
        static void Main()
        {
            Inbound.Inbound.InboundMain();
            Outbound.Outbound.OutboundMain();
        }
    }
}
