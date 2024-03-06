using Newtonsoft.Json;
using SitecoreXM.Logic.Models;

namespace SitecoreXM.Logic.Management.Models
{
    public class UpdateItemResponse
    {
        [JsonProperty("updateItem")]
        public UpdateItemInfo UpdateItem { get; set; }
    }

    public class UpdateItemInfo
    {
        [JsonProperty("item")]
        public ItemData Item { get; set; }
    }

    public class ItemData
    {
        [JsonProperty("itemId")]
        public Guid? ItemId { get; set; }

        [JsonProperty("path")]
        public string? Path { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }
    }
}
