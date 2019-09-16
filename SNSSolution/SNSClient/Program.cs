using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// QUESTIONS
// How to ensure that the responses/requests were sent before printing that they were successful?
// Why does the text take so long to come through? 5+ min 

namespace SNSClient
{
    class Program
    {
        static void Main(string[] args)
        {
            AmazonSimpleNotificationServiceClient client = new AmazonSimpleNotificationServiceClient(Amazon.RegionEndpoint.USEast1);
            string ARN = "arn:aws:sns:us-east-1:271640224276:DepartureGatechangedTopic";
            string phoneNum = ReadPassengerNumber();
            SubscribeToGateChangeTopic(phoneNum, client, ARN);
            SimulateGateChange(client, ARN);
            Console.ReadLine();
        }

        private static void SimulateGateChange(AmazonSimpleNotificationServiceClient client, string ARN)
        {
            PublishRequest publishRequest = new PublishRequest
            {
                TopicArn = ARN,
                Message = "Departure gate changed."
            };

            try
            {
                PublishResponse publishResponse = client.Publish(publishRequest);
                Console.WriteLine(publishRequest.Message);
                Console.WriteLine("Passengers were notified");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void SubscribeToGateChangeTopic(string phoneNum, AmazonSimpleNotificationServiceClient client, string ARN)
        {
            SubscribeRequest subscribeRequest = new SubscribeRequest
            {
                TopicArn = ARN,
                Protocol = "sms",
                Endpoint = phoneNum
            };

            try
            {
                SubscribeResponse subscribeResponse = client.Subscribe(subscribeRequest);
                Console.WriteLine("You will be notified of any flight changes.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private static string ReadPassengerNumber()
        {
            Console.Write("Enter passenger phone #: ");
            return Console.ReadLine();
        }
    }
}
