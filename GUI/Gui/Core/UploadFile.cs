using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Amazon.S3;
using Amazon.S3.Model;

namespace Gui.Core {
    public class UploadFile {
        static string GetRandomString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            
            StringBuilder sb = new StringBuilder();
            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
            {
                while (sb.Length != length)
                {
                    byte[] oneByte = new byte[1];
                    provider.GetBytes(oneByte);
                    char character = (char)oneByte[0];
                    if (valid.Contains(character.ToString()))
                    {
                        sb.Append(character);
                    }
                }
            }
            return sb.ToString();
        }

        private static string bucketName = "404buckengine";
        public static async Task<string> Upload(string filePath) {
            var client = new AmazonS3Client(Amazon.RegionEndpoint.USWest2);

            try {
                string mimeType = MimeMapping.GetMimeMapping(filePath);

                var key = GetRandomString(10);
                PutObjectRequest putRequest = new PutObjectRequest {
                    BucketName = bucketName,
                    Key = key,
                    FilePath = filePath,
                    ContentType = mimeType
                };

                await client.PutObjectAsync(putRequest);
                
                return $"https://{bucketName}.s3.us-west-2.amazonaws.com/{key}";
            }
            catch (AmazonS3Exception amazonS3Exception) {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                     ||
                     amazonS3Exception.ErrorCode.Equals("InvalidSecurity"))) {
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else {
                    throw new Exception("Error occurred: " + amazonS3Exception.Message);
                }
            }
        }
    }
}