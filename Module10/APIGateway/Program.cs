using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace APIGateway
{
    class Program
    {
        static void Main(string[] args)
        {
            const string API_URL = @"https://3worz0erwc.execute-api.us-east-1.amazonaws.com/test";
            const string CONTENTTYPE = @"application/json";

            try
            {
                string input = "{\"name\":\"thomas\"}";
                long inputLength = input.Length;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(API_URL);
                request.ContentType = CONTENTTYPE;
                request.Method = "POST";

                request.ContentLength = inputLength;
                StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
                requestWriter.Write(input);
                requestWriter.Close();

                WebResponse response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader responseReader = new StreamReader(stream))
                    {
                        string retval = responseReader.ReadToEnd();
                        Console.WriteLine(retval);
                        responseReader.Close();
                    }

                    stream.Close();
                }

            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("Error: " + ex.Message);
            }

            Console.ReadLine();
        }
    }
}
