using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;

namespace Company.Function
{
    public class GetResumeCounter
    {
        private readonly ILogger _logger;
        private readonly CosmosClient _cosmosClient;
        private readonly string _containerName = "counter";
        private readonly string _databaseName = "azureresume";

        public GetResumeCounter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetResumeCounter>();

            // ✅ Your actual Cosmos DB connection string
            string connectionString = Environment.GetEnvironmentVariable("CosmosDBConnectionString");
            
            _cosmosClient = new CosmosClient(connectionString);
        }

        [Function("GetResumeCounter")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            var responseData = req.CreateResponse(HttpStatusCode.OK);
            responseData.Headers.Add("Content-Type", "application/json");

        try
        {
            var container = _cosmosClient.GetContainer(_databaseName, _containerName);

            const string id = "1";
            const string partitionKey = "1";
             Counter counter;

            try
            {
                _logger.LogInformation("Attempting to read item from Cosmos DB...");
                var response = await container.ReadItemAsync<Counter>(id, new PartitionKey(partitionKey));
                counter = response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Item not found, creating new counter.");
                counter = new Counter { id = id, count = 0 };
                await container.CreateItemAsync(counter, new PartitionKey(partitionKey));
            }

            counter.count += 1;
            await container.UpsertItemAsync(counter, new PartitionKey(partitionKey));

            _logger.LogInformation($"Returning updated count: {counter.count}");

            await responseData.WriteStringAsync(JsonConvert.SerializeObject(counter), Encoding.UTF8);
    }
    catch (Exception ex)
    {
        _logger.LogError("❌ ERROR: {Message} \nStackTrace: {StackTrace}", ex.Message, ex.StackTrace);
        responseData.StatusCode = HttpStatusCode.InternalServerError;
        var errorDetails = new
        {
            error = "Internal Server Error",
            message = ex.Message,           // 👈 include this
            stackTrace = ex.StackTrace,      // 👈 and this
            count =0
        };
        await responseData.WriteStringAsync(JsonConvert.SerializeObject(errorDetails), Encoding.UTF8);
    }

    return responseData;
}
    }
}