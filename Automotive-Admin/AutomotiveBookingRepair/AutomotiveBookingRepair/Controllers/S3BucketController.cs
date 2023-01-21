using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon; // for linking your AWS account
using Amazon.S3; // for s3 bucket
using Amazon.S3.Model; // s3 object style
using Microsoft.Extensions.Configuration; // to connect with appsetting.json file because our key is there
using System.IO; // read file
using Microsoft.AspNetCore.Http; // to upload object(not a text) on network or image transmit to network
using System.Net.Mime;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authorization;

namespace AutomotiveBookingRepair.Controllers
{
    public class S3BucketController : Controller
    {
        private const string bucketname = "automotivebookingrepairtp059484";

        // function 1: get the keys from appsetting.json file
        private List<string> getKeys() //copy the key in the list type and then store as s string and then return to getkeys function
        {
            List<string> keys = new List<string>();
            //1. link to appsetting.json file and get back the values
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build(); // to build your file

            //2. collect the key value from appsettings.json
            keys.Add(configure["awscredential:accesskey"]);
            keys.Add(configure["awscredential:secretkey"]);
            keys.Add(configure["awscredential:tokenkey"]);
            return keys;
        }

        // function 2: connect
        private AmazonS3Client getConnect()
        {
            List<string> keys = getKeys();
            AmazonS3Client clientconnect = new AmazonS3Client(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);
            return clientconnect;
        }

        // fucntion 3: i will make sure that i will make a upload file page using index file page. thats why use index in below method
        [Authorize(Roles = "Admin")]
        public IActionResult Index(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                ViewBag.msg = msg;

                if(msg == "Success")
                    ViewBag.color = "Green";
                else
                    ViewBag.color = "Red";
            }
            else
            {
                ViewBag.color = "";
            }
            return View();
        }

        //fucntion 4: to upload image to S3
        public async Task<IActionResult> ProcessUploadImage(List<IFormFile> imagefile)
        {
            string message = "Success";

            AmazonS3Client clientconnect = getConnect();

            //one by one file reading
            foreach(var image in imagefile)
            {
                if(image.ContentType.ToLower() != "image/png" && image.ContentType.ToLower() != "image/jpeg" &&
                    image.ContentType.ToLower() != "image/gif")
                {
                    return RedirectToAction("Index", "S3Bucket", new { msg = "Not a correct image file!" });
                }
                try
                {
                    //put the image to the S3 bucket
                    PutObjectRequest request = new PutObjectRequest
                    {
                        InputStream = image.OpenReadStream(), // to tell where i can get the input stream, that is why open read stream
                        BucketName = bucketname+"/images",     // where you want to locate
                        Key = image.FileName,
                        CannedACL = S3CannedACL.PublicRead   // to ensure that image is not private
                    };
                    // execute the request
                    await clientconnect.PutObjectAsync(request);
                }
                catch(AmazonS3Exception ex)
                {
                    return RedirectToAction("Index", "S3Bucket", new { msg = ex.Message });
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Index", "S3Bucket", new { msg = ex.Message });
                }
            }
            return RedirectToAction("Index", "S3Bucket", new { msg = message});
        }

        // function 5: display images from s3 as a picture gallery
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DisplayImageFromS3()
        {
            AmazonS3Client clientconnect = getConnect(); // call the getconnnect function here

            // now i need to display
            // create a list to store the image list (s3object model)
            List<S3Object> imagesList = new List<S3Object>();
            try
            {
                // token here is acutually 
                string token = null;

                do
                {
                    //create list object request to the s3
                    ListObjectsRequest request = new ListObjectsRequest
                    {
                        BucketName = bucketname
                    };

                    // getting response (images) back from the s3
                    ListObjectsResponse response = await clientconnect.ListObjectsAsync(request).ConfigureAwait(false); // to execute the request and keep it in request
                    imagesList.AddRange(response.S3Objects); // store whole data structure of s3object (single node)
                    token = response.NextMarker; // nextmarker is same as Linkedlist concept, here it is next address
                }
                while (token != null);
            }
            catch(AmazonS3Exception ex) // focus on S3 bucket side
            {
                return BadRequest("Error" + ex.Message);
            }
            catch (Exception ex) // focus on all technical error in client side including network issues
            {
                return BadRequest("Error" + ex.Message);
            }
            return View(imagesList); // bring the imageslist to frontend
        }

        //function 6: delete selected image from S3 bucket
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> deleteImage(string ImageName)
        {
            AmazonS3Client clientconnect = getConnect();

            //start deleting 
            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest
                {
                    BucketName = bucketname,
                    Key = ImageName
                };
                // run the request
                await clientconnect.DeleteObjectAsync(request);
            }
            catch (AmazonS3Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }

            return RedirectToAction("DisplayImageFromS3", "S3Bucket");
        }

        // fucntion 7: download image back from s3 bucket
        public async Task<IActionResult> DownloadImage(string ImageName)
        {
            AmazonS3Client clientconnect = getConnect();
            //string mimeType = "application/unknown";
            //string ext = System.IO.Path.GetExtension(ImageName).ToLower();
            //Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext); // for windows
            //if (regkey != null && regkey.GetValue("Content Type") != null)
            //    mimeType = regkey.GetValue("Content Type").ToString();
            const string DefaultContentType = "application/octet-stream";
            var provider = new FileExtensionContentTypeProvider();
            if(!provider.TryGetContentType(ImageName, out string contentType))
            {
                contentType = DefaultContentType;
            }
            Stream imageStream;

            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketname,
                    Key = ImageName
                };
                GetObjectResponse response = await clientconnect.GetObjectAsync(request);

                using (var responseStream = response.ResponseStream)
                {
                    imageStream = new MemoryStream();
                    await responseStream.CopyToAsync(imageStream);
                    imageStream.Position = 0;
                }
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception("Error Message: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Message: " + ex.Message);
            }

            string imageFile = Path.GetFileName(ImageName);

            Response.Headers.Add("Content-Dispostion", new ContentDisposition
            {
                FileName = imageFile,
                Inline = true // true mean direct view in browser, if false direct download on PC
            }.ToString());

            return File(imageStream, contentType);
        }

        // fucntion 8: get pre-signed url image
        public IActionResult GetPresignedURLImage(string ImageName)
        {
            AmazonS3Client clientconnect = getConnect();
            ViewBag.PresignedURL = "";

            try
            {
                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
                {
                    BucketName = bucketname,
                    Key = ImageName,
                    Expires = DateTime.Now.AddMinutes(1) // expire after 1 minute
                };
                ViewBag.PresignedURL = clientconnect.GetPreSignedURL(request); // string url
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception("Error Message" + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Message" + ex.Message);
            }
            return View();
        }
    }
}
