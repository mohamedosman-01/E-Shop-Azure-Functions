using AzureFunctions.Dtos;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
namespace AzureFunctions.Functions;

public class DeliveryOrderProcessor
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    public DeliveryOrderProcessor(ILoggerFactory loggerFactory, IConfiguration configuration)
    {
        _logger = loggerFactory.CreateLogger<DeliveryOrderProcessor>();
        _configuration = configuration;
    }
    [Function("DeliveryOrderProcessor")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString("Welcome to Azure Functions!");


        string requestBody = string.Empty;
        using (StreamReader streamReader = new StreamReader(req.Body))
        {
            requestBody = await streamReader.ReadToEndAsync();
        }
        var order = JsonConvert.DeserializeObject<OrderDto>(requestBody);
        order.PartitionKey = order.Id;
        // Create a new instance of the Cosmos Client
        var cosmosClient = new CosmosClient(_configuration["EndpointUri-CosmosDB"], _configuration["PrimaryKey-CosmosDB"], new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });
        Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync("ordersdb");
        Container container = await database.CreateContainerIfNotExistsAsync("order-container", "/partitionKey");
        var andersenFamilyResponse = await container.CreateItemAsync(order, new PartitionKey(order.PartitionKey));
        return response;

    }
}
