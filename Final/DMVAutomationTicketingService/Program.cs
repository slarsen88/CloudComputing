using System;
using System.Collections.Generic;
using System.Xml;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace DMVAutomationTicketingService
{
    class Program
    {
        static void Main(string[] args)
        {
            List<DMVDatabase> listOfEntries;
            listOfEntries =  ParseXML();
            DetectLicensePlates(listOfEntries);
            Console.WriteLine("finished");
            //NotifyDriverOfViolation(listOfEntries);
            Console.ReadLine();
        }

        private static void DetectLicensePlates(List<DMVDatabase> listOfEntries)
        {
            DMVDatabase driver = new DMVDatabase();
            string bucketName = "cs455final-images";
            // list of photos
            List<string> listOfPhotos = new List<string>();
            listOfPhotos.Add("Car1.jpg");
            listOfPhotos.Add("Car2.jpg");
            listOfPhotos.Add("Car3.jpg");
            listOfPhotos.Add("Car4.png");
            listOfPhotos.Add("Car5.jpg");

            AmazonRekognitionClient rekogClient = new AmazonRekognitionClient(Amazon.RegionEndpoint.USEast1);
            foreach (string photo in listOfPhotos)
            {
                DetectTextRequest textRequest = new DetectTextRequest
                {
                    Image = new Image
                    {
                        S3Object = new S3Object
                        {
                            Bucket = bucketName,
                            Name = photo
                        }
                    }
                };

                DetectTextResponse textResponse = rekogClient.DetectText(textRequest);
                foreach (var textLabel in textResponse.TextDetections)
                {
                    if (textLabel.DetectedText.Contains('-'))
                    {
                        string licensePlate = textLabel.DetectedText;
                        foreach (DMVDatabase dbEntry in listOfEntries)
                        {
                            if (dbEntry.VehiclePlate.Equals(licensePlate) && dbEntry.HasViolation == false)
                            {
                                dbEntry.HasViolation = true;
                            }
                        }
                    }
                }
            }
            
            NotifyDriverOfViolation(listOfEntries);
        }

        private static void NotifyDriverOfViolation(List<DMVDatabase> listOfEntries)
        {
            AmazonSimpleNotificationServiceClient snsClient = new AmazonSimpleNotificationServiceClient(Amazon.RegionEndpoint.USEast1);
            foreach (DMVDatabase driver in listOfEntries)
            {
                Console.WriteLine("Has been notified: " + driver.HasBeenNotified + " Has violation: " + driver.HasViolation);
                if (!driver.HasBeenNotified && driver.HasViolation)
                {
                    PublishRequest pubRequest = new PublishRequest
                    {
                       PhoneNumber = driver.Phone,
                        Message = driver.Name + ", you have a ticket that needs to be paid"
                    };

                    snsClient.PublishAsync(pubRequest);
                    Console.WriteLine("Request to notify by email sent");
                    driver.HasBeenNotified = true;
                }

                //System.Threading.Thread.Sleep(5000);
            }

        }

        // Function that parses XML and stores data into a list of DMVDatabase objects
        private static List<DMVDatabase> ParseXML()
        {
            List<DMVDatabase> listOfDMVEntries = new List<DMVDatabase>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("DMVDatabase.xml");
            XmlNode rootElement = xmlDoc.DocumentElement;
            XmlNodeList rootsChildren = rootElement.ChildNodes;
            foreach (XmlNode child in rootsChildren)
            {
                DMVDatabase dbEntry = new DMVDatabase();
                dbEntry.VehiclePlate = child.Attributes[0].Value;
                dbEntry.Make = child.Attributes[1].Value;
                dbEntry.Model = child.Attributes[2].Value;
                dbEntry.Year = child.Attributes[3].Value;

                foreach (XmlNode grandChild in child.ChildNodes)
                {
                    dbEntry.Name = grandChild.Attributes[0].Value;
                    if (grandChild.InnerText.StartsWith('+'))
                    {
                        dbEntry.Phone = grandChild.InnerText;
                    }
                    //else
                    //{
                    //    dbEntry.Email = grandChild.InnerText;
                    //}
                }
                dbEntry.HasViolation = false;
                listOfDMVEntries.Add(dbEntry);
            }

            return listOfDMVEntries;
        }
    }
}
