using System.Threading.Tasks;
using AwsS3Bucket.Models;

namespace AwsS3Bucket.Interfaces
{
    public interface IS3Service
    {
        Task<CreateS3BucketResponce> CreateBucketAsync(string bucketName);
        Task UploadFileAsync(string bucketName);
    }
}