using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            AmazonKinesisClient client = new AmazonKinesisClient(Amazon.RegionEndpoint.USEast1);

            for(int i = 0; i <= 9; i++)
            {
                string message = "this is input number: " + i;
                byte[] bytes = Encoding.ASCII.GetBytes(message);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    PutRecordRequest requestRecord = new PutRecordRequest
                    {
                        StreamName = "LogsStream",
                        PartitionKey = i.ToString(),
                        Data = ms
                    };
                    PutRecordResponse responseRecord = client.PutRecord(requestRecord);
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
