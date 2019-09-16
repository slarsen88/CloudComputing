using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Translate;
using Amazon.Translate.Model;

namespace Translation
{
    class Program
    {
        static void Main(string[] args)
        {
            AmazonTranslateClient client = new AmazonTranslateClient(Amazon.RegionEndpoint.USEast1);
            string userSentence = sentenceToTranslate();
            while (!userSentence.Equals("q"))
            {
                translateText(userSentence, client);
                Console.WriteLine();
                userSentence = sentenceToTranslate();
            }
            Console.WriteLine("Program ended");
            Console.ReadLine();
        }

        private static void translateText(string userSentence, AmazonTranslateClient client)
        {
            // Danish
            // Italian
            // French
            // Spanish
            string[] languageCodes = { "da", "it", "fr", "es" };
            string[] languages = { "DANISH", "ITALIAN", "FRENCH", "SPANISH" };
            TranslateTextRequest[] requests = new TranslateTextRequest[4];

            // Create a list of request objects 
            for (int i = 0; i < 4; i++)
            {
                TranslateTextRequest request = new TranslateTextRequest
                {
                    SourceLanguageCode = "en",
                    TargetLanguageCode = languageCodes[i],
                    Text = userSentence
                };

                requests[i] = request;
            }
            try
            {
                int i = 0;
                foreach (var r in requests)
                {
                    TranslateTextResponse response = client.TranslateText(r);
                    Console.WriteLine(languages[i] + ":  " + response.TranslatedText);
                    i++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        // Function to prompt user to enter a sentence and return that sentence
        private static string sentenceToTranslate()
        {
            Console.Write("Enter a sentece (or q to quit): ");
            return Console.ReadLine().ToLower();           
        }
    }
}
