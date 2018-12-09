using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AwsS3Bucket.Models
{
    public class CreateS3BucketResponce
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}
