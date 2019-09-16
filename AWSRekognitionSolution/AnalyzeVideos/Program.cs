using Amazon;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* RESOURCES
 *
 * https://docs.aws.amazon.com/rekognition/latest/dg/API_StartLabelDetection.html?shortFooter=true
 * https://docs.aws.amazon.com/rekognition/latest/dg/API_GetLabelDetection.html?shortFooter=true
 *
 */

namespace AnalyzeVideos
{
    class Program
    {
        // Create client
        static AmazonRekognitionClient client = new AmazonRekognitionClient(Amazon.RegionEndpoint.USEast1);
        static string bucketName = "cs4552019-videos";
        static string startJobId = string.Empty;
        static List<VideoEntity> listOfVideoObjects = new List<VideoEntity>();


        static void Main(string[] args)
        {
            List<string> videoNames = new List<string>();
            videoNames.Add("Buildings.mp4");
           //videoNames.Add("Horse.mp4");
            videoNames.Add("ManPlayingGuitar.mp4");
            //videoNames.Add("Street1.mp4");
            //videoNames.Add("Street2.mp4");
            //videoNames.Add("StudentsInCoffeeShop.mp4");   
            //videoNames.Add("StudentsWalking.mp4");
            videoNames.Add("WomenInGym.mp4");
            StartAnalyizingVideos(client, videoNames);
            List<VideoEntity> labels = CheckAnalyzeStatus();
            DisplayLabelsFound(labels);
            Console.WriteLine("donezo");
            Console.ReadLine();
        }

        private static void DisplayLabelsFound(List<VideoEntity> labels)
        {
            Console.WriteLine("Done analyzing all videos." + "\n" + "Printing results: \n" + "-----------------");
            foreach (VideoEntity video in labels)
            {
                Console.WriteLine(video.Name + "\n");
                foreach (var label in video.Labels)
                {
                    Console.WriteLine(label.Label.Name + " (" + label.Label.Confidence + ")");
                }
                Console.WriteLine();
            }
        }

        private static void StartAnalyizingVideos(AmazonRekognitionClient client, List<string> videoNames)
        {
            foreach (string video in videoNames)
            {
                StartLabelDetectionRequest startLabelDetectionRequest = new StartLabelDetectionRequest()
                {
                    Video = new Video
                    {
                        S3Object = new S3Object
                        {
                            Bucket = bucketName,
                            Name = video
                        }
                    },
                    MinConfidence = 90
                };

                StartLabelDetectionResponse response = client.StartLabelDetection(startLabelDetectionRequest);
                startJobId = response.JobId;
                VideoEntity videoEntity = new VideoEntity();
                videoEntity.ID = response.JobId;
                videoEntity.Name = video;
                videoEntity.IsAnalyzed = false;
                listOfVideoObjects.Add(videoEntity);
            }

            Console.WriteLine("Analysis of " + listOfVideoObjects.Count + " videos started. Waiting 45 seconds before checking for results...");
        }

        private static List<VideoEntity> CheckAnalyzeStatus()
        {
            List<VideoEntity> videoLabels = new List<VideoEntity>();
            int sizeOfList = listOfVideoObjects.Count;
            System.Threading.Thread.Sleep(15000);
            do
            {
                foreach (VideoEntity video in listOfVideoObjects)
                {

                    GetLabelDetectionRequest getLabelDetectionRequest = new GetLabelDetectionRequest
                    {
                        JobId = video.ID,
                        MaxResults = 5
                    };

                    GetLabelDetectionResponse getLabelDetectionResponse = client.GetLabelDetection(getLabelDetectionRequest);
                    string status = getLabelDetectionResponse.JobStatus;
                    if (status.Equals("IN_PROGRESS"))
                    {
                        Console.WriteLine(video.Name + ": Analysis in progress...");
                    }
                    else if (status.Equals("SUCCEEDED") && video.IsAnalyzed == false)
                    {
                        Console.WriteLine(video.Name + ":  Analysis succeeded");
                        video.IsAnalyzed = true;
                        video.Labels = getLabelDetectionResponse.Labels;
                        videoLabels.Add(video);
                       
                        sizeOfList--;
                    }
                }

                if (sizeOfList == 0)
                {
                    break;
                }
                Console.WriteLine();
                //wait 45 seconds and loop again
                System.Threading.Thread.Sleep(15000);
            } while (sizeOfList > 0);

            return videoLabels;
        }
    }
}
