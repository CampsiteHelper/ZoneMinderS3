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

		static RateLimiter rl = new RateLimiter("s3upload",30);
      
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

            U.log($"Creating s3 client to endpoint {endPoint.DisplayName}");

            var cred = new BasicAWSCredentials(Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"), Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY"));


            S3Client = new AmazonS3Client(cred, endPoint);



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

            var resp = S3Client.PutObjectAsync(request);
            try
            {


                U.log($"preflight testing s3 -  upload of {path} to s3://test.txt", "preflight");



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
                if (resp != null && resp.Result!=null)
                {
                    if(resp.Result.ResponseMetadata!=null && resp.Result.ResponseMetadata.Metadata.Count >0)
                    {
                        foreach(var rm in resp.Result.ResponseMetadata.Metadata)
                        {
                            U.log($"key: {rm.Key} val: {rm.Value})");
                            
                        }
                            
                    }

                }
                return false;

            }

            U.log("Preflight succeeded");

            return true;






        }





        public void uploadFile(string filepath, string s3path)
        {


			while(rl.checkRate("s3upload"))
			{
				System.Threading.Thread.Sleep(500);
				
			}
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


                    U.log($"Uploading {filepath} to s3://{request.Key}", "uploadFile");

                    var resp = S3Client.PutObjectAsync(request);

                    if (!String.IsNullOrEmpty(U.config["TO_ADDDRESS"]))
                    {
                        SendSESEmail.sendMail("Alarm", filepath);
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
