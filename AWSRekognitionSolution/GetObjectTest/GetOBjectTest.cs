using Amazon;
using Amazon.Rekognition;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetObjectTest
{
    class GetObjectTest
    {
        // Create client
        static AmazonRekognitionClient client = new AmazonRekognitionClient(Amazon.RegionEndpoint.USEast1);
        //static string bucketName = "cs455-videos";
        static string bucketName = "cc3b04a-f041-41c0-a8bb-3ce0d4ae2e2d";
        static string video = "Object_name.mp4";
        static string startJobId = string.Empty;

        // TESTING FOR READING BUCKET OBJECTS
        static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        static IAmazonS3 s3Client;
        static void Main(string[] args)
        {
            s3Client = new AmazonS3Client(bucketRegion);
            ReadObjectDataAsync();
            Console.ReadLine();
        }

        private static void ReadObjectDataAsync()
        {
            string responseBody = "";
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = video
                };

                GetObjectResponse response = s3Client.GetObject(request);
                string title = response.Metadata["x-amz-meta-title"];
                string contentType = response.Headers["Content-Type"];
                Console.WriteLine("Object metadata, Title: {0}", title);
                Console.WriteLine("Content type: {0}", contentType);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered***. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message: '{0}' when writing an object", e.Message);
            }
        }
    }
}
