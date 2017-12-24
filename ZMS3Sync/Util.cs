using System;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;

namespace ZMS3Sync
{
    public static class U
    {
        public static IConfigurationRoot config;

        public static void log(string message,[CallerMemberName] string callerName = "")
        {


            Console.WriteLine(DateTime.Now.ToString($"yyyy-MM-ddTHH\\:mm\\:ss.fff")+ $"- {callerName} - {message}");


        }

        public static void log(string message, Exception e, [CallerMemberName] string callerName = "")
        {

            log(message + " " + e.ToString() + " " + e.StackTrace, callerName);

        }

        public static void initConfig()
        {

            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

             config = builder.Build();

            //export the settings an env variables
            foreach(var c in config.AsEnumerable())
            {
                if(!string.IsNullOrEmpty(c.Value))
                 Environment.SetEnvironmentVariable(c.Key,c.Value);

            }




        }

    }
}
