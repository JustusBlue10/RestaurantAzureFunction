using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;
using Azure;
using Azure.Storage.Blobs;
using System.Text;

namespace MenuFunction
{
    // C# record type for items in the table
    public record Product : ITableEntity
    {
        public string RowKey { get; set; } = default!;

        public string PartitionKey { get; set; } = default!;

        public string Name { get; init; } = default!;

        //public string BlobURL { get; init; } = default!;

        public int Quantity { get; init; }

        public bool Sale { get; init; }

        public ETag ETag { get; set; } = default!;

        public DateTimeOffset? Timestamp { get; set; } = default!;
    }

    public static class TableFunction
    {
        [FunctionName("TableFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
          
            // Storage connection
            var CONNECTION_STRING = Environment.GetEnvironmentVariable("AZURE_CONN_STRING");

            // New instance of the TableClient class
            TableServiceClient tableServiceClient = new TableServiceClient(CONNECTION_STRING);

            // New instance of TableClient class referencing the server-side table
            TableClient tableClient = tableServiceClient.GetTableClient(
                tableName: "kukhnyata-na-yasen"
            );

            await tableClient.CreateIfNotExistsAsync();


            // Trigger stuff
            
                Console.WriteLine("This would be your table");

                var rowKeyID = Guid.NewGuid().ToString();

                // Blob stuff
                BlobServiceClient blobServiceClient = new BlobServiceClient(CONNECTION_STRING);

                const string BLOB_CONTAINER = "orders";
                // string BlobUri = rowKeyID;


                // You probably don't want to use a StreamWriter in this scenario ...
                var filename = rowKeyID;
                byte[] byteArray = Encoding.ASCII.GetBytes(requestBody);
                MemoryStream stream = new MemoryStream(byteArray);
                Azure.Storage.Blobs.BlobClient blobClient = new Azure.Storage.Blobs.BlobClient(
                connectionString: CONNECTION_STRING,
                blobContainerName: BLOB_CONTAINER,
                blobName: $"{filename}.json");


                // upload the file
                blobClient.Upload(stream);

                // Create new item using composite key constructor
                var prod1 = new Product()
                {
                    RowKey = rowKeyID,     // Combination of RowKey and PartitionKey always needs to be unique
                    PartitionKey = "recent-orders",
                    Name = data.Name,
                    Quantity = data.Quantity,
                    Sale = data.Sale
                };

                // Add new item to server-side table
                await tableClient.AddEntityAsync<Product>(prod1);
            

            bool succes = new BlobClient(CONNECTION_STRING, BLOB_CONTAINER, $"{filename}.json").Exists();

            if (succes)
            {
                return new OkObjectResult("Order stored succesfully");
            } else
            {
                return new OkObjectResult("Error: could not save the order");
            }

           
        }
    }
}
