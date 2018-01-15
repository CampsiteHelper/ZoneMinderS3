using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ZMS3Sync
{
    class Program
    {
        static int Main(string[] args)
        {

      
            U.log("Starting up and getting configuration");

            U.initConfig();

            if (!checkConfig())
            {
                return -1;
            }
            U.log("Configuration check complete");

            ZoneMinderEvent.getEvents();

            return 99;


        }


        private static bool checkConfig()
        {

            var rv = true;

         
            if(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID")))
            {
                U.log("ERROR - No AWS_ACCESS_KEY_ID environment value set");
                rv = false;
            }

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY")))
            {
                U.log("ERROR - No AWS_SECRET_ACCESS_KEY environment value set");
                rv = false;
            }

            if(string.IsNullOrEmpty(U.config["S3Region"]))
            {
                U.log("ERROR - No S3Region config value set");
                rv = false;
                
            }

            if (string.IsNullOrEmpty(U.config["S3Bucket"]))
            {
                U.log("ERROR - No S3Bucket config value set");
                return false;

            }


            if (!rv)
                return false;



            ///finally preflight
            var s3u = new S3Uploader(U.config["S3Region"], U.config["S3Bucket"]);

            return s3u.preflight();



        }


    }
}
