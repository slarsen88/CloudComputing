using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectObjectsInImage
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> filePaths = new List<string>();
            filePaths.Add("CoupleWithDog.jpg");
            filePaths.Add("HouseWithCar.jpg");
            filePaths.Add("KidsPlayingOnstreet.jpg");
            filePaths.Add("ManOnBike.jpg");
            filePaths.Add("TwoDogsRunning.jpg");
            filePaths.Add("building_pond.jpg");
            filePaths.Add("chairs.jpg");
            filePaths.Add("pond_people.jpg");

            foreach (var photo in filePaths)
            {
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Failed to load file " + photo);
                }


                AmazonRekognitionClient client = new AmazonRekognitionClient(Amazon.RegionEndpoint.USEast1);
                DetectLabelsRequest request = new DetectLabelsRequest
                {
                    Image = image,
                    MinConfidence = 90f
                };

                try
                {
                    Console.WriteLine("Image : " + photo + "\n");
                    DetectLabelsResponse response = client.DetectLabels(request);
                    foreach (Label label in response.Labels)
                    {
                        Console.WriteLine("\t{0} : {1}", label.Name, label.Confidence);
                    }

                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.ReadLine();

        }
    }
}
