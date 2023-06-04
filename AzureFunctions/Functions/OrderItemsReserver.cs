using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
namespace AzureFunctions.Functions;

public class OrderItemsReserver
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    public OrderItemsReserver(ILoggerFactory loggerFactory, IConfiguration configuration)
    {
        _configuration = configuration;
        _logger = loggerFactory.CreateLogger<OrderItemsReserver>();
    }

    [Function("Function1")]
    public async Task Run([ServiceBusTrigger(queueName: "%ContainerName%", Connection = "ServiceBus")] string myQueueItem)
    {
        _logger.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        var blobClient = new BlobContainerClient(_configuration["AzureBlobStorageOrders"], _configuration["ContainerName"]);
        var blob = blobClient.GetBlobClient($"{Guid.NewGuid()}.json");
        // convert string to stream
        byte[] byteArray = Encoding.UTF8.GetBytes(myQueueItem);
        using MemoryStream stream = new MemoryStream(byteArray);
        await blob.UploadAsync(stream);
    }
}
