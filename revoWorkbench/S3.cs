using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace revoWorkbench
{
    public class S3
    {
        string temp = Path.GetTempPath();
        //const string accesskey = "AKIAJ433F7AKPW2D7GZA";
        //const string secretkey = "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";
        AmazonS3Client client;
        
        public S3()
        {
            this.client = new AmazonS3Client(System.Configuration.ConfigurationSettings.AppSettings["s3accessKey"], System.Configuration.ConfigurationSettings.AppSettings["s3screctKey"], Amazon.RegionEndpoint.USWest2);     
        }

        public void bucketsLists()
        {
            var buckets = this.client.ListBuckets();
        }

        public void bucketElements(string bucketName)
        {
            var elements = this.client.ListObjects(bucketName);
        }
        public void downloadFile(string bucketName,string fileName)
        {
            GetObjectRequest request = new GetObjectRequest();
            request.BucketName = bucketName;
            request.Key = fileName;
            GetObjectResponse response = client.GetObject(request);
            //GetObjectResponse debresponse = client.GetObject(request);
            response.WriteResponseStreamToFile(temp+"userView.json");
            //debresponse.WriteResponseStreamToFile(System.Configuration.ConfigurationSettings.AppSettings["path"] +"\\bin\\debug\\userView.json");
        }

    }
}
