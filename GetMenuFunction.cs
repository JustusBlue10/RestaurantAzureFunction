using System;
using System.IO;
using Azure.Storage.Blobs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs.Models;



namespace function_res
{
    public static class GetMenuFunction
    {
        [FunctionName("GetMenuFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            var connection = Environment.GetEnvironmentVariable("AZURE_CONN_STRING");


            const string BLOB_CONTAINER = "$web";


            string BlobUri = "menucontent.json";
            var blob = new BlobClient(connection, BLOB_CONTAINER, BlobUri);                                                        // check file download was success or no
            var content = await blob.OpenReadAsync(); // I don't know what you want to do with this
            StreamReader reader = new StreamReader(content);
            string RawMenu = reader.ReadToEnd();


            return new OkObjectResult(RawMenu);
        }
    }
}