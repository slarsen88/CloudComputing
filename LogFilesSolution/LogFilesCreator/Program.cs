using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFilesCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] names = { "Dave", "John", "Ralph", "Gertrude", "Olga", "Kevin", "Fatma", "Shaila", "Beverly", "Joey", "Alexa", "Matt", "Derek", "Ana", "Alex"};
            string[] accessPoint = { "PC", "MAC", "iPhone", "Android", "Tablet"};
            string[] errorCodes = { "07-30", "12-12", "55-55", "200" };
            string[] activity = { "Browsing", "Saving", "Chatting", "Shopping"};
            string[] newUser = { "true", "false" };
            Random rand = new Random();
            
            
            string path = @"C:\Users\stuar\Desktop\logFiles\logFile5.txt";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        int randomNameNumber = rand.Next(0, 15);
                        int randomAccessPointNumber = rand.Next(0, 5);
                        int randomErrorCodesNumber = rand.Next(0, 4);
                        int randomActivityNumber = rand.Next(0, 4);
                        int randomNewUserNumber = rand.Next(0, 2);
                        sw.Write(names[randomNameNumber] + ", " + accessPoint[randomAccessPointNumber] + ", " + errorCodes[randomErrorCodesNumber] + ", " + activity[randomActivityNumber] + ", " + newUser[randomNewUserNumber]);
                        sw.WriteLine();
                    }
                }
            }
        }
    }
}
