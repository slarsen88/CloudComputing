using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace EnvVariablesFunction
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
            string apiURL = Environment.GetEnvironmentVariable("API_URL");
            string tableName = Environment.GetEnvironmentVariable("TABLE_NAME");

            Console.WriteLine("apiURL = {0}", apiURL);
            Console.WriteLine("tableName = {0}", tableName);

            return input?.ToUpper();
        }
    }
}
