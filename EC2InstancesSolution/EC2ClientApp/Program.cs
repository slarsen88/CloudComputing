using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC2ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {

            var ec2Client = new AmazonEC2Client(RegionEndpoint.USEast1);
            ListCustomAMI(ec2Client);
            ListAllInstances(ec2Client);
            string IDToDelete = GetInstancesFromCustomAMI(ec2Client);
            DeleteInstancesFromCustomAMI(IDToDelete, ec2Client);


            Console.ReadLine();
        }

        private static void DeleteInstancesFromCustomAMI(string iDToDelete, AmazonEC2Client ec2Client)
        {
            var deleteRequest = new TerminateInstancesRequest()
            {
                InstanceIds = new List<string>() { iDToDelete }
            };

            var deleteResponse = ec2Client.TerminateInstances(deleteRequest);
            foreach (InstanceStateChange item in deleteResponse.TerminatingInstances)
            {
                Console.WriteLine("\nTerminating Instances: " + item.InstanceId);
            }
        }

        private static void ListCustomAMI(AmazonEC2Client ec2Client)
        {
            DescribeImagesRequest request = new DescribeImagesRequest()
            {
                Owners = new List<string>()
                {
                    "271640224276"
                }
            };

            Console.WriteLine("My AMIs:\n-----");
            DescribeImagesResponse response = ec2Client.DescribeImages(request);
            foreach (var r in response.Images)
            {
                Console.WriteLine(r.ImageId + ", " + r.Name + ", " + r.Architecture);
            }
            
        }

        private static void ListAllInstances(AmazonEC2Client ec2Client)
        {
            var request = new DescribeInstancesRequest();
            var response = ec2Client.DescribeInstances(request);
            int count = response.Reservations.Count;
            Console.WriteLine("\nInstances:\n-----");

            for (int i = 0; i < count; i++)
            {
                foreach (var r in response.Reservations[i].Instances)
                {
                    Console.WriteLine(r.InstanceId + " (" + r.ImageId + "), " + r.Platform + ", status = " + r.State.Name);
                }
            }
        }

        private static string GetInstancesFromCustomAMI(AmazonEC2Client ec2Client)
        {
            DescribeImagesRequest request = new DescribeImagesRequest()
            {
                Owners = new List<string>()
                {
                    "271640224276"
                }
            };

            DescribeImagesResponse response = ec2Client.DescribeImages(request);
            string AMIid = string.Empty;
            foreach (var r in response.Images)
            {
                AMIid = r.ImageId;
            }

            string instanceID = string.Empty;

            var instanceRequest = new DescribeInstancesRequest();
            var instanceResponse = ec2Client.DescribeInstances(instanceRequest);
            int count = instanceResponse.Reservations.Count;
            for (int i = 0; i < count; i++)
            {
                foreach (var r in instanceResponse.Reservations[i].Instances)
                {
                    if (r.ImageId == AMIid)
                    {
                        instanceID = r.InstanceId;
                    }
                }
            }
            return instanceID;
        }
    }
}
