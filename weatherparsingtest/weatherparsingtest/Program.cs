using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weatherparsingtest
{
    class Program
    {
        static void Main(string[] args)
        {
            string rawData = "23.4, 150., 33.1, 9.0";
            string[] data = rawData.Split(new char[] { ',' });
            PrintData(data);
            Console.ReadLine();
        }

        private static void PrintData(string[] data)
        {
            foreach (string s in data)
            {
                double d = Convert.ToDouble(s);
                if (d < 10.0)
                {
                    d = 10.0;
                }
                else if (d > 80.0)
                {
                    d = 80.0;
                }

                Console.WriteLine(d);
            }
        }
    }
}
