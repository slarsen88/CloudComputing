using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using Amazon.S3.Util;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace VideoRekognition
{
    public class Function
    {
        static AmazonRekognitionClient client = new AmazonRekognitionClient(Amazon.RegionEndpoint.USEast1);

        IAmazonS3 S3Client { get; set; }

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {
            S3Client = new AmazonS3Client();
        }

        /// <summary>
        /// Constructs an instance with a preconfigured S3 client. This can be used for testing the outside of the Lambda environment.
        /// </summary>
        /// <param name="s3Client"></param>
        public Function(IAmazonS3 s3Client)
        {
            this.S3Client = s3Client;
        }
        
        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
        /// to respond to S3 notifications.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(S3Event evnt, ILambdaContext context)
        {
            var s3Event = evnt.Records?[0].S3;
            if(s3Event == null)
            {
                return null;
            }

            try
            {
                var response = await this.S3Client.GetObjectMetadataAsync(s3Event.Bucket.Name, s3Event.Object.Key);
                var key = s3Event.Object.Key;
                var bucketName = s3Event.Bucket.Name;
                await StartAnalyzingVideosAsync(key, bucketName);
                return response.Headers.ContentType;
            }
            catch(Exception e)
            {
                context.Logger.LogLine($"Error getting object {s3Event.Object.Key} from bucket {s3Event.Bucket.Name}. Make sure they exist and your bucket is in the same region as this function.");
                context.Logger.LogLine(e.Message);
                context.Logger.LogLine(e.StackTrace);
                throw;
            }
        }

        public async Task StartAnalyzingVideosAsync(string key, string bucketName)
        {
            string startJobID = string.Empty;
            StartLabelDetectionRequest startLabelDetectionRequest = new StartLabelDetectionRequest
            {
                Video = new Video
                {
                    S3Object = new S3Object
                    {
                        Bucket = bucketName,
                        Name = key
                    }
                },
                MinConfidence = 50
            };

            var startLabelDetectionResponse = await client.StartLabelDetectionAsync(startLabelDetectionRequest);
            if (startLabelDetectionResponse != null)
            {
                startJobID = startLabelDetectionResponse.JobId;
            }

            Console.WriteLine("Analysis of {0} has started. Waiting 20 seconds to check status.", key);
            GetLabelDetectionRequest labelDetectionRequest = new GetLabelDetectionRequest()
            {
                JobId = startJobID
            };

            System.Threading.Thread.Sleep(20000);
            bool isPackage = false;
            bool isComplete = false;
            // while job is not "successful"
            while (!isComplete)
            {
                GetLabelDetectionResponse labelDetectionResponse = await client.GetLabelDetectionAsync(labelDetectionRequest);
                if (labelDetectionResponse.JobStatus == VideoJobStatus.SUCCEEDED)
                {
                    isComplete = true;
                    Console.WriteLine("Job status is: " + labelDetectionResponse.JobStatus);
                    foreach (var label in labelDetectionResponse.Labels)
                    {
                        if (label.Label.Name.Equals("Package Delivery") || label.Label.Name.Equals("Carton") || label.Label.Name.Equals("Box"))
                        {
                            isPackage = true;
                            break;
                        }                 
                    }
                }

                if (isPackage)
                {
                    break;
                }
                Console.WriteLine("Still analyzing...");
                System.Threading.Thread.Sleep(20000);
            }

            AmazonSimpleNotificationServiceClient snsClient = new AmazonSimpleNotificationServiceClient(Amazon.RegionEndpoint.USEast1);

            if (isPackage)
            {
                string ARN = "arn:aws:sns:us-east-1:207457486405:PackageDelivery";
                PublishRequest pubRequest = new PublishRequest
                {
                    TopicArn = ARN,
                    Message = "You have a delivery."

                };
                await snsClient.PublishAsync(pubRequest);
                Console.WriteLine("You have a delivery!");
            }
            else
            {
                Console.WriteLine("Someone's at your door but there is no delivery");
            }
        }
    }
}
