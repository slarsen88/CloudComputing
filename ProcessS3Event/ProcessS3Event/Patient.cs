using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessS3Event
{
    class Patient
    {
        public string id { get; set; }
        public string age { get; set; }
        public string gender { get; set; }
        public string maritalStatus { get; set; }
        public string bmi { get; set; }
        public string smoker { get; set; }
        public string alcoholConsumtion { get; set; }
        public Test[] tests { get; set; }
        public string hasVascularDisease { get; set; }
    }
}
