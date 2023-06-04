
using Newtonsoft.Json;

namespace AzureFunctions.Dtos;
public class OrderDto
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "partitionKey")]
    public string PartitionKey { get; set; }
    public List<ItemDto> Items { get; set; }
    public AddressDto Address { get; set; }
}

