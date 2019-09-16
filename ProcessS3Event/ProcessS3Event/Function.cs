using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Util;
using System.Xml;
using System.IO;
using System.Text;
using Npgsql;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ProcessS3Event
{
    public class Function
    {
        IAmazonS3 S3Client { get; set; }

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {
            S3Client = new AmazonS3Client();
        }

        /// <summary>
        /// Constructs an instance with a preconfigured S3 client. This can be used for testing the outside of the Lambda environment.
        /// </summary>
        /// <param name="s3Client"></param>
        public Function(IAmazonS3 s3Client)
        {
            this.S3Client = s3Client;
        }
        
        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
        /// to respond to S3 notifications.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(S3Event evnt, ILambdaContext context)
        {
            // Create variable to hold the open connection
            var openDBConnection = OpenConnection();
            CreateTable(openDBConnection);
            
            var s3Event = evnt.Records?[0].S3;
            if (s3Event == null)
            {
                return null;
            }
            
            string bucketName = s3Event.Bucket.Name;
            string objectKey = s3Event.Object.Key;

            Stream stream = await S3Client.GetObjectStreamAsync(bucketName, objectKey, null);
            using (StreamReader reader = new StreamReader(stream))
            {
                string content = reader.ReadToEnd();
                // Check if document starts with '<' or '{'. That will determine if the file type is XML or JSON
                if (content.StartsWith('<'))
                {
                    var xmlList = ParseXML(content);
                    InsertIntoTable(xmlList, openDBConnection);
                }
                else
                {
                    var jsonList = ParseJSON(content);
                    InsertIntoTable(jsonList, openDBConnection);
                }

                openDBConnection.Close();
                return bucketName;
            }
        }

        // Create the connection to the DB and return the open connection
        private NpgsqlConnection OpenConnection()
        {
            string connString = @"Server=pginstance.ckdseatgshlm.us-east-1.rds.amazonaws.com;" +
                "Port=5432;" +
                "Database=testdb;" +
                "User Id=cs455user;" +
                "Password=cs455pass;" +
                "Timeout=15";

            try
            {
                NpgsqlConnection conn = new NpgsqlConnection(connString);
                conn.Open();
                Console.WriteLine("Call to conn.Open() returned");

                if (conn.State == System.Data.ConnectionState.Open)
                    return conn;
                else
                    Console.WriteLine("Connection to PostgreSQL database testdb failed to open");
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("ERROR opening connection to testdb: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR opening connection to testdb: " + ex.Message);
            }
                       
            return null;
        }

        // Function receives the open connection and creates a table if it does not already exist
        private void CreateTable(NpgsqlConnection conn)
        {
            Console.WriteLine("CreateTable Called");
            var createTable = new NpgsqlCommand
            {
                CommandText = "CREATE TABLE IF NOT EXISTS PatientsTests(ID CHAR(256) CONSTRAINT id PRIMARY KEY, " +
                               "age CHAR(256), " +
                               "gender CHAR(256), " +
                               "maritalStatus CHAR(256), " +
                               "bmi CHAR(256), " + 
                               "smoker CHAR(256), " +
                               "alcoholConsumption CHAR(256), " +
                               "totalcholesterol CHAR(256), " +
                               "LDLcholesterol CHAR(256), " +
                               "HDLcholesterol CHAR(254), " +
                               "Triglycerides CHAR(256), " +
                               "plasmaCermaides CHAR(256), " + 
                               "natriueticPeptide CHAR(256), " + 
                               "hasVascularDisease CHAR(256))"                      
            };
            createTable.Connection = conn;
            createTable.ExecuteNonQuery();

        }

        // Function receives a string containing XML data and parses it, returning a list of XML data
        private List<string> ParseXML(string content)
        {
            List<string> xmlList = new List<string>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content);
            XmlElement rootElement = xmlDoc.DocumentElement;
            XmlNodeList rootsChildren = rootElement.ChildNodes;
            foreach (XmlNode rootChild in rootsChildren)
            {
                if (rootChild.Name.Equals("tests"))
                {
                    Console.WriteLine(rootChild.Name);
                    XmlNodeList testsChildren = rootChild.ChildNodes;
                    foreach (XmlNode testChild in testsChildren)
                    {
                        xmlList.Add(testChild.InnerText);
                    }
                }
                else
                {
                    xmlList.Add(rootChild.InnerText);
                }
            }

            return xmlList;
        }

        // Function receives a string containing JSON data and parses it, returning a list of JSON data
        private List<string> ParseJSON(string content)
        {
            List<string> jsonList = new List<string>();
            Patient p1 = JsonConvert.DeserializeObject<Patient>(content);
            if (p1 != null)
            {
                jsonList.Add(p1.id);
                jsonList.Add(p1.age);
                jsonList.Add(p1.gender);
                jsonList.Add(p1.maritalStatus);
                jsonList.Add(p1.bmi);
                jsonList.Add(p1.smoker);
                jsonList.Add(p1.alcoholConsumtion);
                foreach (Test t in p1.tests)
                {
                    jsonList.Add(t.value);
                }
                jsonList.Add(p1.hasVascularDisease);
            }

            return jsonList;
        }

        // Function takes in list of data (XML or JSON) and the open connection, and inserts that data into the DB
        private void InsertIntoTable(List<string> list, NpgsqlConnection conn)
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO PatientsTests (ID, age, gender, maritalStatus, bmi, smoker, alcoholConsumption, totalcholesterol, LDLcholesterol, HDLcholesterol, Triglycerides, plasmaCermaides, natriueticPeptide, hasVascularDisease)" +
                                                "VALUES (@id, @age, @gender, @maritalStatus, @bmi, @smoker, @alcoholConsumption, @totalcholesterol, @LDLcholesterol, @HDLcholesterol, @Triglycerides, @plasmaCermaides, @natriueticPeptide, @hasVascularDisease)", conn))
            {
                cmd.Parameters.AddWithValue("id", list[0]);
                cmd.Parameters.AddWithValue("age", list[1]);
                cmd.Parameters.AddWithValue("gender", list[2]);
                cmd.Parameters.AddWithValue("maritalStatus", list[3]);
                cmd.Parameters.AddWithValue("bmi", list[4]);
                cmd.Parameters.AddWithValue("smoker", list[5]);
                cmd.Parameters.AddWithValue("alcoholConsumption", list[6]);
                cmd.Parameters.AddWithValue("totalcholesterol", list[7]);
                cmd.Parameters.AddWithValue("LDLcholesterol", list[8]);
                cmd.Parameters.AddWithValue("HDLcholesterol", list[9]);
                cmd.Parameters.AddWithValue("Triglycerides", list[10]);
                cmd.Parameters.AddWithValue("plasmaCermaides", list[11]);
                cmd.Parameters.AddWithValue("natriueticPeptide", list[12]);
                cmd.Parameters.AddWithValue("hasVascularDisease", list[13]);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
