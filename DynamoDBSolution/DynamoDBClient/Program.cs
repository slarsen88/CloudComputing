using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace DynamoDBClient
{
    class Program
    {
        static void Main(string[] args)
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
            DynamoDBContext context = new DynamoDBContext(client);
            

            ListTables(client);
            GetItemsByArtist(context, "No One You Know");
            GetItemsByArtist(context, "The Acme Band");

          
            SaveItem(context);
            Console.ReadLine();
        }

        private static void ListTables(AmazonDynamoDBClient client)
        {
            Console.WriteLine("Tables:\n--------\n");
            foreach (var x in client.ListTables().TableNames)
            {
                Console.WriteLine(x + "\n");
            }
          
        }

        private static void GetItemsByArtist(DynamoDBContext context, string artist)
        {
            Console.WriteLine("Items for artist '" + artist + "'\n");
           // Music musicItem = context.Load<Music>(artist, "Call Me Today");
            var musicItem = context.Query<Music>(artist);
            foreach (var x in musicItem)
            {
                Console.WriteLine("Artist: " + artist);
                Console.WriteLine("SongTitle: " + x.songTitle);
                if (x.Year != null)
                {
                    Console.WriteLine("Year: " + x.Year);
                }
                if (x.Awards != null)
                {
                    Console.Write("Awards: ");
                    foreach (var award in x.Awards)
                    {
                        Console.Write(award + " ");
                    }
                    Console.WriteLine();
                }
                if (x.RecordCompany != null)
                {
                    Console.WriteLine("RecordCompany: " + x.RecordCompany);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static void SaveItem(DynamoDBContext context)
        {
            Music item = new Music()
            {
                Artist = "Elton John",
                Awards = new List<string> {"Top 10"},
                songTitle = "Tiny Dancer",
                Year = "1987",
                RecordCompany = "Warner Brothers"
            };

            context.Save(item);

            Music savedSong = context.Load<Music>(item.Artist, item.songTitle);
            Console.WriteLine("Added a new song by " + item.Artist);
        }
    }
}
