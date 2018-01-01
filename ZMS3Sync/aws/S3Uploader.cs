using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace ZMS3Sync
{
    public class S3Uploader
    {
        IAmazonS3 S3Client { get; set; }
        static TaskFactory factory { get; set; }

        static Object lockObj = new object();

        string bucket;
        Amazon.RegionEndpoint endPoint;

        public S3Uploader(string Region, string Bucket)
        {

            endPoint = Amazon.RegionEndpoint.GetBySystemName(Region);
            bucket = Bucket;
            var maxConcur = U.config["MaxConcurrentUploads"] ?? "10";

            maxconcurrency = Convert.ToInt32(maxConcur);


            //set up the limited concurrency
            lock (lockObj)
            {
                if (factory == null)
                {
                    LimitedConcurrencyLevelTaskScheduler lcts = new LimitedConcurrencyLevelTaskScheduler(maxconcurrency);
                    factory = new TaskFactory(lcts);
                }
                   
            }
            S3Client = new AmazonS3Client(endPoint);


        }

        static int maxconcurrency = 10;

        public bool preflight()
        {

            var path = Path.GetTempFileName();

            var sw = File.CreateText(path);

            sw.WriteLine("Hello Testing");
            sw.Close();

            var request = new PutObjectRequest()
            {
                BucketName = bucket,
                Key = "test.txt",
                FilePath = path,
                StorageClass = S3StorageClass.StandardInfrequentAccess,


            };


            try
                {
                    

                U.log($"preflight testing s3 -  upload of {path} to s3://test.txt", "preflight" );

                    var resp = S3Client.PutObjectAsync(request);
                  
                    resp.Wait();

                   

                    if (resp.Result.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    {
                        U.log($"Error {resp.Result.HttpStatusCode} with request ");
                    return false;

                    }

             
                }
                catch (Exception e)
                {
                    
                    U.log("Error in uploadFile", e, "uploadFile");
                return false;

                }

            U.log("Preflight succeeded");

            return true;






        }





        public void uploadFile(string filepath, string s3path)
        {

           
            var fname = Path.GetFileName(filepath);

        
            var request = new PutObjectRequest()
            {
                BucketName = bucket,
                Key = s3path + "/" + fname,
                FilePath = filepath,
                StorageClass = S3StorageClass.StandardInfrequentAccess,


            };



            factory.StartNew(() =>
            {
                try
                {
                    

                    U.log($"Uploading {filepath} to s3://{request.Key}", "uploadFile" );

                    var resp = S3Client.PutObjectAsync(request);
                  
                    if(!String.IsNullOrEmpty(U.config["TO_ADDDRESS"]))
                    {
                        SendSESEmail.sendMail("Alarm",filepath);
                    }
                    resp.Wait();

                   

                    if (resp.Result.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    {
                        U.log($"Error {resp.Result.HttpStatusCode} with request ");
                    }
                }
                catch (Exception e)
                {
                    
                    U.log("Error in uploadFile", e, "uploadFile");
                }
              


            });


        }



    }
}
