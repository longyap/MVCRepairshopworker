using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace AutomotiveBookingRepair.Controllers
{
    public class SNSController : Controller
    {
        private const string topicARN = "arn:aws:sns:us-east-1:025401065428:SNStp059484";

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
        private AmazonSimpleNotificationServiceClient getConnect()
        {
            List<string> keys = getKeys();
            AmazonSimpleNotificationServiceClient clientconnect = new AmazonSimpleNotificationServiceClient(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);
            return clientconnect;
        }

        //function 3: create subscription page -> subscribe any message from a topic(receive the broadcast once subscribe)
        public IActionResult Index(string ? msg)
        {
            ViewBag.msg = msg; // to receive msg. to show msg, go on index 
            return View();
        }

        // fucntion 4: register email in the subscription cornor in SNS topic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> processSubscription(string emailAddress)
        {
            string message = "success register email in the subscription corner!";
            AmazonSimpleNotificationServiceClient clientconnect = getConnect();

            try
            {
                SubscribeRequest request = new SubscribeRequest
                {
                    TopicArn = topicARN,
                    Protocol = "email",
                    Endpoint = emailAddress
                };

                var response = await clientconnect.SubscribeAsync(request);
                var requestID = response.ResponseMetadata.RequestId;
                message = message + "\nYour register ID is " + requestID;
            }
            catch(AmazonSimpleNotificationServiceException ex)
            {
                message = "Error Message: " + ex.Message;
            }

            return RedirectToAction("Index", "SNS", new { msg = message });
        }

        //function 5: create an email broadcasting page for admin side
        [Authorize(Roles = "Admin")]
        public IActionResult BroadcastMessagePage(string ? msg)
        {
            ViewBag.msg = msg;
            return View();
        }

        //function 6: help to process the braodcast email message
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> processBroadcastMessage(string SubjectTitle, string broadcastMessage)
        {
            string message = "Your message has been delivered to the subscriber now!" + 
                "Please ask your subscriber to check their email now";

            AmazonSimpleNotificationServiceClient clientconnect = getConnect();
            try
            {
                //publish message to email
                PublishRequest request = new PublishRequest
                {
                    TopicArn = topicARN,
                    Message = broadcastMessage,
                    Subject = SubjectTitle
                };
                PublishResponse response =  await clientconnect.PublishAsync(request);

            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                message = "Error Message: " + ex.Message;
            }
            catch (Exception ex)
            {
                message = "Error Message: " + ex.Message;
            }

            return RedirectToAction("BroadcastMessagePage", "SNS", new { msg = message });
        }

    }
}
