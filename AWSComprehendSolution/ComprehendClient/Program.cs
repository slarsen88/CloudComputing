using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Amazon.Comprehend;
using Amazon.Comprehend.Model;

namespace ComprehendClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Log into client with credentials
            AmazonComprehendClient client = new AmazonComprehendClient(Amazon.RegionEndpoint.USEast1);
           
            // Load XML doc
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("phrases_old.xml");
            XmlElement rootElement = xmlDoc.DocumentElement;
            XmlNodeList rootsChildren = rootElement.ChildNodes;

            // List of language codes
            List<string> languageCodes = new List<string>();

            // Iterate over each child and send request to detect language for inner text of each node
            foreach (XmlNode node in rootsChildren)
            {
                DetectDominantLanguageRequest request = new DetectDominantLanguageRequest
                {
                    Text = node.InnerText
                };

                DetectDominantLanguageResponse response = client.DetectDominantLanguage(request);

                string test = string.Empty;
                           
                // Add language found for each child statement to list
                foreach (DominantLanguage language in response.Languages)
                {
                    test += language.LanguageCode + ", ";                   
                }
                languageCodes.Add(test);

            }

            // print results of language found
            int i = 0;
            foreach (XmlNode node in rootsChildren)
            {
                Console.Write("Phrase " + node.Attributes[0].Name + ": " + node.Attributes[0].Value + "      ");
                Console.WriteLine(node.Attributes[1].Value + " : " + languageCodes[i].Substring(0, languageCodes[i].Length - 2));
                i++;
            }

            Console.ReadLine();
        }
    }
}
