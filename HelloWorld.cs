using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MenuFunction
{
    public static class HelloWorld
    {
        [FunctionName("HelloWorld")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            //var menuJson = File.ReadAllText("C:\\Dev\\MenuFunction\\menucontent.json");

            var menuJson = @"[

    {""id"":0,""Dish"":""Smoked Salmon"",""Price"":6.25},

    {""id"":1,""Dish"":""Carrot Soup"",""Price"":3.55},

    {""id"":2,""Dish"":""Chicken Balerno"",""Price"":9.35},

    {""id"":3,""Dish"":""Roast Beef"",""Price"":10.00},

    {""id"":4,""Dish"":""Pizza Americana"",""Price"":10.20},

    {""id"":5,""Dish"":""Chocolate gateau"",""Price"":4.00},

    {""id"":6,""Dish"":""Chocolate Cake"",""Price"":4.50},

    {""id"":7,""Dish"":""Coffee and Mints"",""Price"":2.50},

    {""id"":8,""Dish"":""Margarita lemon"",""Price"":7},

    {""id"":9,""Dish"":""Cosmopolitan"",""Price"":7.99},

    {""id"":10,""Dish"":""Moscow Mule"",""Price"":3.99}

]";

            return new OkObjectResult(menuJson);
        }
    }
}
