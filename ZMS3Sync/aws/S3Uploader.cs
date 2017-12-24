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

        string bucket;
        Amazon.RegionEndpoint endPoint;

        public S3Uploader(string Region, string Bucket)
        {

            endPoint = Amazon.RegionEndpoint.GetBySystemName(Region);
            bucket = Bucket;

        }

        public void uploadFile(string filepath, string s3path)
        {
            
            IAmazonS3 client;

            var fname =  Path.GetFileName(filepath);

            client = new AmazonS3Client(endPoint);

            var request = new PutObjectRequest()
            {
                BucketName = bucket,
                Key = s3path + "/" + fname,
                FilePath = filepath,
                StorageClass = S3StorageClass.StandardInfrequentAccess
           };



            Task.Factory.StartNew(()=>{
                try
                {
                    U.log($"Uploading {filepath}","uploadFile");
                    
                   var resp =  client.PutObjectAsync(request);
                    resp.Wait();
                    if(resp.Result.HttpStatusCode!=System.Net.HttpStatusCode.OK)
                    {
                        U.log($"Error {resp.Result.HttpStatusCode} with request ");
                    }
                }
                catch(Exception e)
                {
                    U.log("Error in uploadFile",e,"uploadFile");
                }


            });


         }



    }
}
