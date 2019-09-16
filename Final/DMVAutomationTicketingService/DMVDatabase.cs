using System;
using System.Collections.Generic;
using System.Text;

namespace DMVAutomationTicketingService
{
    class DMVDatabase
    {
        public string VehiclePlate { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public bool HasBeenNotified { get; set; }
        public bool HasViolation { get; set; }
    }
}
