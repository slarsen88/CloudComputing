using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace DynamoDBClient
{
    [DynamoDBTable("Music")]
    class Music
    {
        [DynamoDBHashKey]
        public string Artist { get; set; }
        public List<string> Awards { get; set; }
        public string songTitle { get; set; }
        public string Year { get; set; }
        public string RecordCompany { get; set; }
    }
}
