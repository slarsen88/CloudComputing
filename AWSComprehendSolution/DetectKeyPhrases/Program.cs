using Amazon.Comprehend;
using Amazon.Comprehend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DetectKeyPhrases
{
    class Program
    {
        static void Main(string[] args)
        {
            // Log into client with credentials
            AmazonComprehendClient client = new AmazonComprehendClient(Amazon.RegionEndpoint.USEast1);

            // Load XML doc
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("phrases.xml");
            XmlElement rootElement = xmlDoc.DocumentElement;
            XmlNodeList rootsChildren = rootElement.ChildNodes;

            foreach (XmlNode node in rootsChildren)
            {
                DetectKeyPhrasesRequest request = new DetectKeyPhrasesRequest
                {
                    Text = node.InnerText,
                    LanguageCode = "en"
                };

                DetectKeyPhrasesResponse response = client.DetectKeyPhrases(request);
                Console.WriteLine(node.InnerText);
                foreach (KeyPhrase kp in response.KeyPhrases)
                {
                    
                    Console.Write("\t" + kp.Text + " (" + kp.Score + ") \n");
                }
                Console.WriteLine();

                
            }

            Console.ReadLine();
        
        }
    }
}
