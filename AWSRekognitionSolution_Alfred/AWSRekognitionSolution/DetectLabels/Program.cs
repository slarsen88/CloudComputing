using System;
using System.IO;

using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

using Amazon.Rekognition;
using Amazon.Rekognition.Model;

namespace DetectLabels
{
    class Program
    {
        static void Main(string[] args)
        {
            String photo = @"C:\Temp\banner.png";

            Image image = new Image();
            try
            {
                using (FileStream fs = new FileStream(photo, FileMode.Open, FileAccess.Read))
                {
                    byte[] data = null;
                    data = new byte[fs.Length];
                    fs.Read(data, 0, (int)fs.Length);
                    image.Bytes = new MemoryStream(data);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load file " + photo);
                return;
            }

            try
            {
                // Constructs a SharedCredentialsFile object from the default credentials file.
                SharedCredentialsFile sharedCredentialsFile = new SharedCredentialsFile();

                // Get the [default] profile from the credentials file.
                CredentialProfile defaultProfile = GetDefaultProfile(sharedCredentialsFile);

                if (defaultProfile != null)
                {
                    // Get the credentials (access key, secret access key, etc.)
                    AWSCredentials credentials = AWSCredentialsFactory.GetAWSCredentials(defaultProfile, new SharedCredentialsFile());

                    AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient(credentials, RegionEndpoint.USEast1);

                    DetectTextRequest detectTextRequest = new DetectTextRequest()
                    {
                        Image = image,
                    };

                    DetectTextResponse detectTextResponse = rekognitionClient.DetectText(detectTextRequest);
                    foreach(TextDetection td in detectTextResponse.TextDetections)
                    {
                        Console.WriteLine(td.DetectedText);
                    }
                }
                else
                {
                    Console.WriteLine("AWS [default] profile not found");
                }
            }
            catch(AmazonRekognitionException ex)
            {
                Console.WriteLine("AWS Rekognition ERROR: {0}", ex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex.Message);
            }

            Console.ReadLine();
        }

        /// <summary>
        /// Gets the default profile from the credentials file.
        /// </summary>
        /// <param name="sharedCredentialsFile">The credentials file.</param>
        /// <returns>The default profile.</returns>
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
