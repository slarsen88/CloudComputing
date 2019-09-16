using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueClient
{
    class Program
    {
        public static void Main(string[] args)
        {
            AmazonSQSClient client = new AmazonSQSClient(Amazon.RegionEndpoint.USEast1);
            var inputQueue = @"https://sqs.us-east-1.amazonaws.com/207457486405/InputQueue";
            var outputQueue =  @"https://sqs.us-east-1.amazonaws.com/207457486405/OutputQueue";
            while (true)
            {
                ReceiveMessageRequest request = new ReceiveMessageRequest
                {
                    MaxNumberOfMessages = 1,
                    QueueUrl = inputQueue,
                    WaitTimeSeconds = 20
                };

                Console.WriteLine("Waiting for messages...");
                ReceiveMessageResponse response = client.ReceiveMessage(request);
                if (response.Messages.Count > 0)
                {
                    Console.WriteLine(response.Messages[0].Body);
                    var inputMessage = response.Messages[0].Body;
                    MessageEntity message = JsonConvert.DeserializeObject<MessageEntity>(inputMessage);
                    SendMessageToOutputQueue(message, outputQueue, client);

                    // If we send more than 1 message at a time, but I only want to send 1
                    //foreach (var message in response.Messages)
                    //{
                    //    Console.WriteLine(message.Body);
                    //    foreach (var attr in message.Attributes)
                    //    {
                    //        var key = attr.Key;
                    //        Console.WriteLine(key);
                    //    }
                    //   SendMessageToOutputQueue(inputMessage, outputQueue, client);
                    //}
                }
                else
                {
                    Console.WriteLine("No message received");
                }

            }

        }

        private static void SendMessageToOutputQueue(MessageEntity message, string outputQueueURL, AmazonSQSClient client)
        {
            message.message = "reply_message";
            string outputMessage = JsonConvert.SerializeObject(message);
            SendMessageRequest sendMessageRequest = new SendMessageRequest(outputQueueURL, outputMessage);
            SendMessageResponse sendMessageResponse = client.SendMessage(sendMessageRequest);
            Console.WriteLine("Message sent succesfully");
        }

        public static void SendMessageToOutputQueue(string inputMessage, string outputQueueURL, AmazonSQSClient client)
        {
            throw new NotImplementedException();


        }
    }
}
