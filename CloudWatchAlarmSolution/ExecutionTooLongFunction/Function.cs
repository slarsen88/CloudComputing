using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ExecutionTooLongFunction
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(string input, ILambdaContext context)
        {
            Console.WriteLine("Sleepning for 10 seconds...");

            // Sleep for 10,000 milliseconds
            System.Threading.Thread.Sleep(10000);

            Console.WriteLine("Resuming execution...");

            return input?.ToUpper();
        }
    }
}
