using System;
using System.Text;
using System.Collections.Generic;

using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

using Amazon.Rekognition;
using Amazon.Rekognition.Model;

namespace AnalyzeVideos
{
    class Program
    {
        private static VideoItem[] videosList = new VideoItem[]
        {
            new VideoItem("Buildings.mp4"),
            new VideoItem("Horse.mp4"),
            new VideoItem("ManPlayingGuitar.mp4"),
            new VideoItem("Street1.mp4"),
            new VideoItem("Street2.mp4"),
            new VideoItem("StudentsInCoffeeShop.mp4"),
            new VideoItem("StudentsWalking.mp4"),
            new VideoItem("WomenInGym.mp4")
        };

        private static int completeCount = 0;

        static void Main(string[] args)
        {
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

                    foreach(VideoItem vi in videosList)
                    {
                        Video v = new Video()
                        {
                            S3Object = GetVideo(vi.FileName)
                        };
                        StartLabelDetectionRequest request = new StartLabelDetectionRequest()
                        {
                            Video = v,
                            MinConfidence = 90
                        };

                        StartLabelDetectionResponse response = rekognitionClient.StartLabelDetection(request);
                        if(response != null)
                        {
                            vi.JobId = response.JobId;
                        }
                    }

                    Console.WriteLine("Analyses of all {0} videos started. Waiting for 45 seconds before start checking for results...", videosList.Length);
                    Console.WriteLine();

                    // Sleep for 45 seconds
                    System.Threading.Thread.Sleep(45000);

                    while(completeCount < videosList.Length)
                    {
                        foreach (VideoItem vi in videosList)
                        {
                            if (!vi.AnalysisComplete)
                            {
                                GetLabelDetectionRequest labelDetectionRequest = new GetLabelDetectionRequest()
                                {
                                    JobId = vi.JobId,
                                    MaxResults = 5
                                };

                                GetLabelDetectionResponse labelDetectionResponse = rekognitionClient.GetLabelDetection(labelDetectionRequest);

                                StringBuilder sb = new StringBuilder();
                                sb.Append(vi.FileName + ": ");

                                if (labelDetectionResponse.JobStatus == VideoJobStatus.SUCCEEDED)
                                {
                                    vi.AnalysisComplete = true;
                                    vi.AnalysisSucceeded = true;
                                    completeCount++;

                                    Console.ForegroundColor = ConsoleColor.Green;
                                    sb.Append("Analysis succeeded");

                                    vi.Labels = labelDetectionResponse.Labels;
                                }
                                else if (labelDetectionResponse.JobStatus == VideoJobStatus.FAILED)
                                {
                                    vi.AnalysisComplete = true;
                                    vi.AnalysisSucceeded = false;
                                    completeCount++;

                                    Console.ForegroundColor = ConsoleColor.Red;
                                    sb.Append("Analysis failed");
                                }
                                else if (labelDetectionResponse.JobStatus == VideoJobStatus.IN_PROGRESS)
                                {
                                    // Nothing to do
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    sb.Append("Analysis in progress...");
                                }

                                Console.WriteLine(sb.ToString());
                            }

                            if (completeCount == videosList.Length)
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine();
                                Console.WriteLine("Done analyzing all {0} videos.", videosList.Length);
                                Console.WriteLine("Printing results:", videosList.Length);
                                Console.WriteLine("----------------------------------------------------");
                                Console.WriteLine();

                                foreach(VideoItem video in videosList)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    sb.AppendLine(video.FileName + ":");
                                    sb.AppendLine();

                                    foreach(LabelDetection lbl in video.Labels)
                                    {
                                        sb.AppendLine(lbl.Label.Name + " (" + lbl.Label.Confidence + " %)");
                                    }

                                    Console.WriteLine(sb.ToString());
                                }
                                break;
                            }
                            else
                                // Sleep for 5 seconds before checking the next video
                                System.Threading.Thread.Sleep(5000);
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

        private static S3Object GetVideo(string fileName)
        {
            S3Object v = new S3Object()
            {
                Bucket = "cs455-videos",
                Name = fileName
            };

            return v;
        }
    }
}
