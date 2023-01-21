// service purchased
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon; // for linking your aws account
using Amazon.DynamoDBv2;  // for dynamo db
using Amazon.DynamoDBv2.Model; // model for items
using Amazon.DynamoDBv2.DocumentModel; // for json file, how we are going to read json
using Microsoft.Extensions.Configuration; // for appsetting.json file
using System.IO; // for appsetting.json file
using AutomotiveBookingRepair.Models;
using Microsoft.AspNetCore.Authorization;

namespace AutomotiveBookingRepair.Controllers
{
    public class DynamoDBController : Controller
    {
        private string tablename = "automotivebookingrepair"; // need to define table name first

        //function 1: create connection

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
        private AmazonDynamoDBClient getConnect()
        {
            List<string> keys = getKeys();
            AmazonDynamoDBClient dynamoclient = new AmazonDynamoDBClient(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);
            return dynamoclient;
        }

        [Authorize(Roles = "Admin")]
        //function 3: menu page
        public async Task<IActionResult> Index(string ? msg)
        {
            ViewBag.msg = msg;
            List<ServicePurchase> orderlist = await getAllPurchaseInfo();
            return View(orderlist);
        }

        //function 4: create table in dynamo db
        public async Task<IActionResult> createTable()
        {
            string message = tablename +  "is successfully created ";
            AmazonDynamoDBClient DynamoDBClient = getConnect();

            try
            {
                CreateTableRequest tableRequest = new CreateTableRequest
                {
                    TableName = tablename,
                    AttributeDefinitions = new List<AttributeDefinition>() // fixed attribute
                    {
                       // first column
                       new AttributeDefinition // partition key for order table
                       {
                          AttributeName = "CustomerName",
                          AttributeType = "S",
                       },

                       new AttributeDefinition // sort key for order table
                       {
                          AttributeName = "TransactionID",
                          AttributeType = "S",
                       }
                    },

                    // keyschema is for telling, which one is partition key and which one is sort key
                    KeySchema = new List<KeySchemaElement>()
                    {
                         new KeySchemaElement // partition key
                         {
                             AttributeName = "CustomerName",
                             KeyType = "HASH" // partition use HASH technology
                         },
                         new KeySchemaElement // sort key
                         {
                             AttributeName = "TransactionID",
                             KeyType = "RANGE"
                         }
                    },
                    // throughput --> to read and write item
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 5,
                        WriteCapacityUnits = 5
                    }
                };
                await DynamoDBClient.CreateTableAsync(tableRequest); // able to run your table
            }
            catch (AmazonDynamoDBException ex)
            {
                message = "Error Message" + ex.Message; 
            }

            return RedirectToAction("Index", "DynamoDB", new { msg = message });
        }

        //function 5 & 6: learn how to add a new item in DynamoDB
        public IActionResult insertForm(string ? msg)
        {
            ViewBag.msg = msg;
            return View();
        }

        public async Task<IActionResult> processOrder(ServicePurchase order)
        {
            string message = "Successfully added the data in " + tablename;
            AmazonDynamoDBClient DynamoDBClient = getConnect();

            Dictionary<string, AttributeValue> document = new Dictionary<string, AttributeValue>();

            try
            {
                //collect user input
                document["CustomerName"] = new AttributeValue { S = order.CustomerName };
                document["TransactionID"] = new AttributeValue { S = Guid.NewGuid().ToString() };
                document["PurchaseService"] = new AttributeValue { S = order.PurchaseService };
                document["PurchaseAmount"] = new AttributeValue { N = order.PurchaseAmount.ToString() };
                document["PurchaseDate"] = new AttributeValue { S = order.PurchaseDate.ToString() };
                document["PurchaseMethod"] = new AttributeValue { S = order.PurchaseMethod };

                //flexible attribute
                if (order.PaymentDate.ToString() != "1/1/0001 12:00:00 AM" &&
                    !string.IsNullOrEmpty(order.PaymentDate.ToString()))
                {
                    document["PaymentDate"] = new AttributeValue { S = order.PaymentDate.ToString() };
                }
                //put item in dynamo db
                PutItemRequest request = new PutItemRequest()
                {
                    TableName = tablename,
                    Item = document
                };
                await DynamoDBClient.PutItemAsync(request);
            }
            catch (AmazonDynamoDBException ex)
            {
                message = "Error message: " + ex.Message;
            }

            return RedirectToAction("insertForm", "DynamoDB", new { msg = message });
        }

        //function 7: learn how to scan all the item out from the table
        public async Task<List<ServicePurchase>> getAllPurchaseInfo()
        {
            List<ServicePurchase> ListofOrder = new List<ServicePurchase>();
            AmazonDynamoDBClient DynamoDBClient = getConnect();
            try
            {
                ScanRequest request = new ScanRequest
                {
                    TableName = tablename
                };

                ScanResponse response = await DynamoDBClient.ScanAsync(request);

                // read data one by one
                foreach(var item in response.Items)
                {
                    ServicePurchase order = new ServicePurchase();
                    order.CustomerName = item["CustomerName"].S;
                    order.TransactionID = item["TransactionID"].S;
                    order.PurchaseService = item["PurchaseService"].S;
                    order.PurchaseAmount = decimal.Parse(item["PurchaseAmount"].N);
                    order.PurchaseDate = DateTime.Parse(item["PurchaseDate"].S);
                    order.PurchaseMethod = item["PurchaseMethod"].S;

                    if (item.ContainsKey("PaymentDate")) 
                    { 
                        order.PaymentDate = DateTime.Parse(item["PaymentDate"].S);
                    }
                    ListofOrder.Add(order);
                }
                return ListofOrder;
            }
            catch (AmazonDynamoDBException ex)
            {
                return ListofOrder;
            }
        }

        // function 8: how to delete a single data from dynamo db
        public async Task<IActionResult> DeletePage(string Pkey, string Skey)
        {
            string message = "Successfully deleted the data of " + Pkey + "from the dynamoDB table" ;

            AmazonDynamoDBClient DynamoDBClient = getConnect();

            try
            {
                DeleteItemRequest request = new DeleteItemRequest
                {
                    TableName = tablename,
                    Key = 
                    new Dictionary<string, AttributeValue>()
                    {
                        {"CustomerName", new AttributeValue { S = Pkey } },
                        {"TransactionID", new AttributeValue { S = Skey } }
                    }
                };
                await DynamoDBClient.DeleteItemAsync(request);
            }
            catch (AmazonDynamoDBException ex)
            {
                message = "Error Message: " + ex.Message;
            }
            return RedirectToAction("Index", "DynamoDB", new { msg = message });
        }

        //function 9: edit data from dynamo DB
        public async Task<IActionResult> EditPage(string Pkey, string Skey)
        {
            AmazonDynamoDBClient DynamoDBClient = getConnect();
            ServicePurchase order1 = new ServicePurchase();
            try
            {
                QueryRequest request = new QueryRequest
                {
                    TableName = tablename,
                    KeyConditionExpression = "CustomerName = :v_CustomerName and TransactionID = :v_TransactionID",
                    // what is the key type
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    {
                        { ":v_CustomerName", new AttributeValue { S = Pkey}},
                        { ":v_TransactionID", new AttributeValue { S = Skey} }
                    }
                };
                QueryResponse response = await DynamoDBClient.QueryAsync(request);
                // need dictionary to extract item one by one
                Dictionary<string, AttributeValue> item = response.Items[0];
                order1.CustomerName = item["CustomerName"].S;
                order1.TransactionID = item["TransactionID"].S;
                order1.PurchaseService = item["PurchaseService"].S;
                order1.PurchaseAmount = decimal.Parse(item["PurchaseAmount"].N);
                order1.PurchaseDate = DateTime.Parse(item["PurchaseDate"].S);
                order1.PurchaseMethod = item["PurchaseMethod"].S;

                if (item.ContainsKey("PaymentDate"))
                {
                    order1.PaymentDate = DateTime.Parse(item["PaymentDate"].S);
                }
            }
            catch (AmazonDynamoDBException ex)
            {
                return RedirectToAction("Index","DynamoDB", new { msg = "Error Message in editing: " + ex.Message });
            }
            return View(order1);
        }

        //fucntion 10: how to process edit action in DynamoDB
        public async Task<IActionResult> processEditOrder(ServicePurchase order)
        {
            string message = "The data of Customer of " + order.CustomerName + " with Transaction ID " + order.TransactionID +
                "has been updated!";
            AmazonDynamoDBClient DynamoDBClient = getConnect();
            try
            {
                Dictionary<string, AttributeValue> item = new Dictionary<string, AttributeValue>();
                item.Add(":v_PurchaseService", new AttributeValue { S = order.PurchaseService });
                item.Add(":v_PurchaseAmount", new AttributeValue { N = order.PurchaseAmount.ToString() });
                item.Add(":v_PurchaseDate", new AttributeValue { S = order.PurchaseDate.ToString() });
                item.Add(":v_PurchaseMethod", new AttributeValue { S = order.PurchaseMethod });

                string updatestatement = "SET PurchaseAmount = :v_PurchaseAmount, PurchaseDate = :v_PurchaseDate, " +
                    "PurchaseMethod = :v_PurchaseMethod , PurchaseService = :v_PurchaseService";

                if (order.PaymentDate.ToString() != "1/1/0001 12:00:00 AM"
                    && !string.IsNullOrEmpty(order.PaymentDate.ToString()))
                {
                    item.Add(":v_PaymentDate", new AttributeValue { S = order.PaymentDate.ToString() });
                    updatestatement = updatestatement + ", PaymentDate = :v_PaymentDate";
                }    

                UpdateItemRequest request = new UpdateItemRequest
                {
                    TableName = tablename,
                    Key = new Dictionary<string, AttributeValue>()
                    {
                        { "CustomerName", new AttributeValue { S = order.CustomerName } },
                        { "TransactionID", new AttributeValue { S = order.TransactionID } }
                    },
                    ExpressionAttributeValues = item,
                    UpdateExpression = updatestatement
                };
                await DynamoDBClient.UpdateItemAsync(request);
            }
            catch (AmazonDynamoDBException ex)
            {
                message = "Error Message: " + ex.Message;
            }
            return RedirectToAction("Index", "DynamoDB", new { msg = message });
        }
    }
}



//private string tablename = "automotivebookingrepair"; // need to define table name first

//// function 1: get the keys from appsetting.json file
//private List<string> getKeys() //copy the key in the list type and then store as s string and then return to getkeys function
//{
//    List<string> keys = new List<string>();
//    //1. link to appsetting.json file and get back the values
//    var builder = new ConfigurationBuilder()
//                        .SetBasePath(Directory.GetCurrentDirectory())
//                        .AddJsonFile("appsettings.json");
//    IConfigurationRoot configure = builder.Build(); // to build your file

//    //2. collect the key value from appsettings.json
//    keys.Add(configure["awscredential:accesskey"]);
//    keys.Add(configure["awscredential:secretkey"]);
//    keys.Add(configure["awscredential:tokenkey"]);
//    return keys;
//}

////function 2
//// display the values in this index
//public async Task<IActionResult> Index()
//{
//    List<string> keys = getKeys();
//    AmazonDynamoDBClient clientconnect = new AmazonDynamoDBClient(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

//    List<ServicePurchase> orderList = new List<ServicePurchase>();
//    try
//    {
//        ScanRequest request = new ScanRequest
//        {
//            TableName = tablename
//        };

//        ScanResponse response = await clientconnect.ScanAsync(request);

//        //now, read items one by one
//        foreach(var item in response.Items)
//        {
//            ServicePurchase order = new ServicePurchase();
//            order.CustomerName = item["CustomerName"].S;
//            order.TransactionID = item["TransactionID"].S;
//            order.PurchaseService = item["PurchaseService"].S;
//            order.PurchaseAmount = decimal.Parse(item["PurchaseAmount"].S);
//            order.PurchaseDate = DateTime.Parse(item["PurchaseDate"].S);
//            order.PurchaseMethod = item["PurchaseMethod"].S;
//            if (item.ContainsKey("PaymentDate"))
//            {
//                order.PaymentDate = DateTime.Parse(item["PaymentDate"].S);
//            }
//            orderList.Add(order);
//        }
//    }
//    catch (AmazonDynamoDBException ex)
//    {
//        return BadRequest("Error: " + ex.Message);
//    }
//    return View(orderList);
//}

////function 3: menu page
//// insert into dynamodb
//public IActionResult insertForm(string ? msg)
//{
//    ViewBag.msg = msg;
//    return View();
//}

//// function 4
//// process order fucntion
//public async Task<IActionResult> processOrder(ServicePurchase order)
//{
//    string message = "Successfully added the data in " + tablename;
//    List<string> keys = getKeys();
//    AmazonDynamoDBClient clientconnect = new AmazonDynamoDBClient(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

//    Dictionary<string, AttributeValue> document = new Dictionary<string, AttributeValue>();

//    try
//    {
//        //collect the user input 
//        document["CustomerName"] = new AttributeValue { S = order.CustomerName };
//        document["TransactionID"] = new AttributeValue { S = Guid.NewGuid().ToString() };
//        document["PurchaseService"] = new AttributeValue { S = order.PurchaseService };
//        document["PurchaseAmount"] = new AttributeValue { S = order.PurchaseAmount.ToString() };
//        document["PurchaseDate"] = new AttributeValue { S = order.PurchaseDate.ToString() };
//        document["PurchaseMethod"] = new AttributeValue { S = order.PurchaseMethod };

//        //flexible attribute
//        if (order.PaymentDate.ToString() != "1/1/0001 12:00:00 AM" &&
//            !string.IsNullOrEmpty(order.PaymentDate.ToString()))
//        {
//            document["PaymentDate"] = new AttributeValue { S = order.PaymentDate.ToString() };
//        }

//        PutItemRequest request = new PutItemRequest()
//        {
//            TableName = tablename,
//            Item = document
//        };
//        await clientconnect.PutItemAsync(request);
//    }
//    catch (AmazonDynamoDBException ex)
//    {
//        return BadRequest("Error" + ex.Message);
//    }
//    return RedirectToAction("Index", "DynamoDB", new { msg = message });
//}








//// function 2: connect
//private AmazonDynamoDBClient getConnect()
//{
//    List<string> keys = getKeys();
//    AmazonDynamoDBClient clientconnect = new AmazonDynamoDBClient(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);
//    return clientconnect;
//}

//// function 3: Index page --> make it become menu page
//public IActionResult Index(string ? msg)
//{
//    ViewBag.msg = msg;
//    return View();
//}

//// fucntion 4: create table -- service purchase table(customer, purchase date or id) -> customer (partition key) , id (sort key)
//public async Task<IActionResult> CreateTable()
//{
//    string message = tablename + "is successfully created in Dynamo DB";

//    AmazonDynamoDBClient dynamoDBClient = getConnect();
//    try
//    {
//        CreateTableRequest tableRequest = new CreateTableRequest
//        {
//            TableName = tablename,
//            AttributeDefinitions = new List<AttributeDefinition>() // fixed attribute
//            {
//                // first column
//                new AttributeDefinition // partition key for order table
//                {
//                    AttributeName = "CustomerName",
//                    AttributeType = "S",
//                },

//                new AttributeDefinition // sort key for order table
//                {
//                    AttributeName = "TransactionID",
//                    AttributeType = "S",
//                }
//            },

//            // keyschema is for telling, which one is partition key and which one is sort key
//            KeySchema = new List<KeySchemaElement>()
//            {
//                new KeySchemaElement // partition key
//                {
//                    AttributeName = "CustomerName",
//                    KeyType = "HASH" // partition use HASH technology
//                },
//                new KeySchemaElement // sort key
//                {
//                    AttributeName = "TransactionID",
//                    KeyType = "RANGE"
//                }
//            },
//            // throughput --> to read and write item
//            ProvisionedThroughput = new ProvisionedThroughput
//            {
//                ReadCapacityUnits = 5,
//                WriteCapacityUnits = 5
//            }
//        };
//        await dynamoDBClient.CreateTableAsync(tableRequest); // able to run your table
//    }
//    catch (AmazonDynamoDBException ex)
//    {
//        message = "Error Message : " + ex.Message;
//    }
//    return RedirectToAction("Index", "DynamoDB", new { msg = message });
//}