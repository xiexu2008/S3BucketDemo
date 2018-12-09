using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using AwsS3Bucket.Interfaces;
using AwsS3Bucket.Models;

namespace AwsS3Bucket.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _client;

        public S3Service(IAmazonS3 client)
        {
            _client = client;
        }

        public async Task<CreateS3BucketResponce> CreateBucketAsync(string bucketName)
        {
            try
            {
                if (await AmazonS3Util.DoesS3BucketExistAsync(_client, bucketName))
                {
                    return new CreateS3BucketResponce
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        Message = $"Bucket {bucketName} already exists."
                    };
                }

                var putBucketRequest = new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true,
                    CannedACL = S3CannedACL.PublicRead
                };

                var response = await _client.PutBucketAsync(putBucketRequest);

                return new CreateS3BucketResponce
                {
                    StatusCode = response.HttpStatusCode,
                    Message = response.ResponseMetadata.RequestId
                };
            }
            catch (AmazonS3Exception e)
            {
                return new CreateS3BucketResponce
                {
                    StatusCode = e.StatusCode,
                    Message = e.Message
                };
            }
            catch (Exception e)
            {
                return new CreateS3BucketResponce
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = e.Message
                };
            }
        }

        public async Task UploadFileAsync(string bucketName)
        {
            var filePath = @"D:\Xu\Share_xu\xx.txt";
            var uploadWithKey = "UploadWithKey";
            var fileStreamUpload = "FileStreamUpload";
            var advanceUpload = "AdvancedUpload";

            try
            {
                var transferUtility = new TransferUtility(_client);

                // Option 1
                await transferUtility.UploadAsync(filePath, bucketName);

                // Option 2
                await transferUtility.UploadAsync(filePath, bucketName, uploadWithKey);

                // Option 3
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    await transferUtility.UploadAsync(fileStream, bucketName, fileStreamUpload);
                }

                // Option 4
                var transferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    Key = advanceUpload,
                    CannedACL = S3CannedACL.NoACL,
                    FilePath = filePath,
                    StorageClass = S3StorageClass.StandardInfrequentAccess,
                    PartSize = 6 * 1024 * 1024 // 6 MB
                };

                transferUtilityRequest.Metadata.Add("param1", "Value1");
                transferUtilityRequest.Metadata.Add("param2", "Value2");

                await transferUtility.UploadAsync(transferUtilityRequest);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"Error occurred on server due to {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown error occurred on server due to {e.Message}");
                throw;
            }
        }
    }
}
