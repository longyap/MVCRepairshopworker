using Microsoft.AspNetCore.Mvc;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace MVCFlowerShopLab12_1.Controllers
{
    public class S3UploadController : Controller
    {
        private const string bucketName = "njosedive76";

        //create connection
        private List<string> getAwsInformation()
        {
            List<string> keys = new List<string>();
            //collect all keys from the apssetting
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            keys.Add(configuration["AwsCredential:aws_access_key_id"]);
            keys.Add(configuration["AwsCredential:aws_secret_access_key"]);
            keys.Add(configuration["AwsCredential:aws_session_token"]);
            return keys;
        }
        //client object for every action
        private AmazonS3Client createClient()
        {
            List<string> keys = getAwsInformation();
            AmazonS3Client S3objectClient = new AmazonS3Client(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);
            return S3objectClient;
        }
        //function 3: create a upload image form from the index page here
        public IActionResult Index(string? msg)
        {
            ViewBag.msg = msg;
            if (!string.IsNullOrEmpty(msg) && msg == "upload success")
            {
                ViewBag.textColor = "Green";
                ViewBag.bgColor = "LightGreen";
            }
            else if (string.IsNullOrEmpty(msg))
            {
                ViewBag.textColor = "Black";
                ViewBag.bgColor = "Red";
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> processImageUpload(List<IFormFile> imagefiles)
        {
            AmazonS3Client S3objectClient = createClient();
            string message = "upload success";
            foreach (var singleimage in imagefiles)
            {
                //image valodation 
                if (singleimage.ContentType.ToLower() != "image/png" &&
                        singleimage.ContentType.ToLower() != "image/jpeg" &&
                        singleimage.ContentType.ToLower() != "image/gif")
                {
                    message = singleimage.FileName + " is not image file";
                    break;
                }
                try
                {
                    PutObjectRequest request = new PutObjectRequest
                    {
                        InputStream = singleimage.OpenReadStream(),
                        BucketName = bucketName + "/images", // put into image folder
                        Key = singleimage.FileName,
                        CannedACL = S3CannedACL.PublicRead //turn image to public
                    };
                    await S3objectClient.PutObjectAsync(request);

                }
                catch (AmazonS3Exception ex)
                {
                    message = "error A " + ex;
                    break;
                }
                catch (Exception ex)
                {
                    message = "error B " + ex;
                }
            }
            return RedirectToAction("Index", "ImageUpload", new { msg = message });
        }
        //function 5 display image from the s3 bucket
        public async Task<IActionResult> DisplayImagesFromS3()
        {
            AmazonS3Client s3objectClient = createClient();
            List<S3Object> images = new List<S3Object>();
            try
            {
                //to get to know whether still have next item or not
                string token = null;
                do
                {
                    //create request to ask for 1 images from s3
                    ListObjectsRequest request = new ListObjectsRequest()
                    {
                        BucketName = bucketName
                    };
                    ListObjectsResponse response = await s3objectClient.ListObjectsAsync(request);
                    token = response.NextMarker;
                    images.AddRange(response.S3Objects);
                }
                while (token != null);
            }
            catch (AmazonS3Exception ex)
            {
                return BadRequest("Error message:" + ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error message:" + ex.Message);
            }
            return View(images);
        }
        public async Task<IActionResult> DeleteImagesFromS3(string ImageName)
        {
            //commection
            AmazonS3Client S3objectClient = createClient();
            string message = "image deleted";
            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = ImageName
                };
                await S3objectClient.DeleteObjectAsync(request);
            }
            catch (AmazonS3Exception ex)
            {
                return BadRequest("Error message:" + ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error message:" + ex.Message);
            }
            return RedirectToAction("DisplayimagesFromS3", "ImageUpload", new { msg = message });
        }
    }
}
