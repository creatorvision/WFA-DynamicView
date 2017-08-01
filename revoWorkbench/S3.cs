using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace revoWorkbench
{
    public class S3
    {
        const string accesskey = "AKIAJ433F7AKPW2D7GZA";
        const string secretkey = "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";
        AmazonS3Client client;
        
        public S3()
        {
            this.client = new AmazonS3Client(accesskey, secretkey,Amazon.RegionEndpoint.USWest2);     
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
            GetObjectResponse releaseresponse = client.GetObject(request);
            GetObjectResponse debugresponse = client.GetObject(request);
            releaseresponse.WriteResponseStreamToFile(System.Configuration.ConfigurationSettings.AppSettings["path"]+"\\bin\\Release\\userView.json");
            debugresponse.WriteResponseStreamToFile(System.Configuration.ConfigurationSettings.AppSettings["path"] +"\\bin\\Debug\\useView.json");
        }

    }
}
