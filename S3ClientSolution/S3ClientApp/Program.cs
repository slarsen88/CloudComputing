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
            SharedCredentialsFile sharedCredentialsFile = new SharedCredentialsFile();
            CredentialProfile defaultProfile = GetDefaultProfile(sharedCredentialsFile);

            if (defaultProfile != null)
            {
                AWSCredentials credentials = AWSCredentialsFactory.GetAWSCredentials(defaultProfile, new SharedCredentialsFile());
                AmazonS3Client s3Client = new AmazonS3Client(credentials, RegionEndpoint.USEast1);
                DeleteBucketRequest request = new DeleteBucketRequest
                {
                    //How to get this name programatically without hard coding? bucket.name?
                    BucketName = "20411141-0e46-4b95-8074-1e28cb421306"
                };

                if (request.Equals(null))
                {                   
                    DeleteBucketResponse dResponse = s3Client.DeleteBucket(request);
                }

                ListBucketsResponse response = s3Client.ListBuckets();
                foreach (S3Bucket r in response.Buckets)
                {
                    Console.WriteLine(r.BucketName + " created at " + r.CreationDate + " type = " + r.GetType());                                    
                }
            }

            Console.ReadLine();
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
