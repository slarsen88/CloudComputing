using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

namespace S3ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string bucketName = "ee95e59c-10bf-4395-9587-5b07fb99c2b7";
            SharedCredentialsFile sharedCredentialsFile = new SharedCredentialsFile();
            CredentialProfile defaultProfile = GetDefaultProfile(sharedCredentialsFile);

            if (defaultProfile != null)
            {
                AWSCredentials credentials = AWSCredentialsFactory.GetAWSCredentials(defaultProfile, new SharedCredentialsFile());
                AmazonS3Client s3Client = new AmazonS3Client(credentials, RegionEndpoint.USEast1);

                Console.WriteLine("Bucket " + PrintBucketName(s3Client));
                ListBucketObjects(bucketName, s3Client); 
            }

            Console.WriteLine("\nDone listing all S3 objects within the bucket");

            Console.ReadLine();
        }

        private static void ListBucketObjects(string bucketName, AmazonS3Client s3Client)
        {
            ListObjectsV2Request listObjectRequest = new ListObjectsV2Request
            {
                BucketName = bucketName
            };

            ListObjectsV2Response listObjectResponse = new ListObjectsV2Response();
            listObjectResponse = s3Client.ListObjectsV2(listObjectRequest);

            foreach (S3Object s3Obj in listObjectResponse.S3Objects)
            {
                Console.WriteLine("key = " + s3Obj.Key);
                ListObjectMetaData(s3Obj, s3Client, bucketName);
            }
        }

        private static void ListObjectMetaData(S3Object s3Obj, AmazonS3Client s3Client, string bucketName)
        {
           GetObjectMetadataRequest objRequest = new GetObjectMetadataRequest
           {
            BucketName = bucketName,
            Key = s3Obj.Key
           };

           GetObjectMetadataResponse objResponse = s3Client.GetObjectMetadata(objRequest);

            foreach (string key in objResponse.Metadata.Keys)
            {
                Console.WriteLine("\tkey: " + key + " value: " + objResponse.Metadata[key]);
            }
            
        }

       

        private static string PrintBucketName(AmazonS3Client s3Client)
        {
            ListBucketsResponse response = s3Client.ListBuckets();
            S3Bucket bucket = response.Buckets[0];
            return bucket.BucketName;
        }
        

        private static CredentialProfile GetDefaultProfile(SharedCredentialsFile sharedCredentialsFile)
        {
            if (sharedCredentialsFile == null)
            {
                throw new ArgumentNullException("Argument sharedCredentialsFile is null");
            }

            const string DEFAULT_PROFILE = "default";

            foreach (CredentialProfile cp in sharedCredentialsFile.ListProfiles())
            {
                if (String.Compare(cp.Name, DEFAULT_PROFILE, false) == 0)
                {
                    return cp;
                }
            }

            return null;
        }
    }
}
