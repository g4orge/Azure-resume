using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Function
{
    public static class GetResumeCounter
    {
        [FunctionName("GetResumeCounter")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,

            // Use the correct attribute name: "Connection" instead of the obsolete/incorrect one
            [CosmosDB(
                databaseName: "azureresume",
                containerName: "counter", // container name is case-sensitive; ensure it matches exactly
                Connection = "AzureResumeConnectionString",
                Id = "1",
                PartitionKey = "1")] Counter counter,

            [CosmosDB(
                databaseName: "azureresume",
                containerName: "counter",
                Connection = "AzureResumeConnectionString",
                Id = "1",
                PartitionKey = "1")] out Counter updatedCounter,

            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Null check to prevent null reference error if no record is found
            if (counter == null)
            {
                log.LogWarning("Counter not found in database. Initializing a new one.");
                updatedCounter = new Counter { id = "1", Count = 1 };
            }
            else
            {
                counter.Count += 1;
                updatedCounter = counter;
            }

            var jsonToReturn = JsonConvert.SerializeObject(updatedCounter);

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
            };
        }
    }
}
