using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Resources https://docs.aws.amazon.com/rekognition/latest/dg/images-bytes.html?shortFooter=true


namespace DetectTextClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //string filePath = "banner.png";
            string filePath = "banner1.jpg";
            Image image = new Image();
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
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
                Console.WriteLine("Failed to load file " + filePath);
            }

            AmazonRekognitionClient client = new AmazonRekognitionClient(Amazon.RegionEndpoint.USEast1);
            DetectTextRequest request = new DetectTextRequest
            {
                Image = image
            };

            try
            {
                DetectTextResponse response = client.DetectText(request);
                foreach (var text in response.TextDetections)
                {
                    Console.WriteLine(text.DetectedText);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}
