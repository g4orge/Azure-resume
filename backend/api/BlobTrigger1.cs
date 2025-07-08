using System.IO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class BlobTrigger1
    {
        private readonly ILogger _logger;

        public BlobTrigger1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<BlobTrigger1>();
        }

        [Function("BlobTrigger1")]
        public void Run(
            [BlobTrigger("samples-workitems/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob,
            string name)
        {
            _logger.LogInformation($"C# Blob trigger function processed blob\n Name: {name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
