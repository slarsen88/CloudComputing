using System;
using System.IO;
using System.Collections.Generic;

using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

using Amazon.Rekognition;
using Amazon.Rekognition.Model;

namespace DetectObjectsInImage
{
    class Program
    {
        static void Main(string[] args)
        {
            const float MIN_CONFIDENCE = 90F;

            try
            {
                string[] imagesList = GetListOfImages();
                if(imagesList == null || imagesList.Length == 0)
                {
                    Console.WriteLine("No images found in the Images folder");
                    return;
                }

                // Constructs a SharedCredentialsFile object from the default credentials file.
                SharedCredentialsFile sharedCredentialsFile = new SharedCredentialsFile();

                // Get the [default] profile from the credentials file.
                CredentialProfile defaultProfile = GetDefaultProfile(sharedCredentialsFile);

                if (defaultProfile != null)
                {
                    // Get the credentials (access key, secret access key, etc.)
                    AWSCredentials credentials = AWSCredentialsFactory.GetAWSCredentials(defaultProfile, new SharedCredentialsFile());

                    AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient(credentials, RegionEndpoint.USEast1);

                    CompareFacesRequest detectFacesRequest = new CompareFacesRequest()
                    {
                        SourceImage = GetImage(@"C:\Temp\TomCruise1.jpg"),
                        TargetImage = GetImage(@"C:\Temp\TomCruise2.jpg")
                    };

                    CompareFacesResponse response = rekognitionClient.CompareFaces(detectFacesRequest);
                    List<CompareFacesMatch> list = response.FaceMatches;

                    foreach(string filePath in imagesList)
                    {
                        Image image = GetImage(filePath);
                        if (image == null)
                            continue;

                        DetectLabelsRequest detectLabelsRequest = new DetectLabelsRequest()
                        {
                            Image = image,
                            MinConfidence = MIN_CONFIDENCE,
                        };

                        DetectLabelsResponse detectLabelsResponse = rekognitionClient.DetectLabels(detectLabelsRequest);

                        Console.WriteLine("Image: {0}\n", filePath);
                        foreach(Label label in detectLabelsResponse.Labels)
                        {
                            Console.WriteLine("\t{0} ({1})", label.Name, label.Confidence);
                        }

                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("AWS [default] profile not found");
                }
            }
            catch (AmazonRekognitionException ex)
            {
                Console.WriteLine("AWS Rekognition ERROR: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex.Message);
            }

            Console.WriteLine("\nDONE");
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

        private static string[] GetListOfImages()
        {
            return Directory.GetFiles("Images", "*.*");
        }

        private static Image GetImage(String filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    Image image = new Image();
                    byte[] data = null;
                    data = new byte[fs.Length];
                    fs.Read(data, 0, (int)fs.Length);
                    image.Bytes = new MemoryStream(data);
                    return image;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load file " + filePath);
            }

            return null;
        }
    }
}
