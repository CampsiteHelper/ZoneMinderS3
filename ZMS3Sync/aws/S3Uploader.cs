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


        }

        static int maxconcurrency = 10;






        public void uploadFile(string filepath, string s3path)
        {

            IAmazonS3 client;

            var fname = Path.GetFileName(filepath);

            client = new AmazonS3Client(endPoint);

            var request = new PutObjectRequest()
            {
                BucketName = bucket,
                Key = s3path + "/" + fname,
                FilePath = filepath,
                StorageClass = S3StorageClass.StandardInfrequentAccess
            };


            factory.StartNew(() =>
            {
                try
                {
                    

                    U.log($"Uploading {filepath}", "uploadFile");

                    var resp = client.PutObjectAsync(request);
                  
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
