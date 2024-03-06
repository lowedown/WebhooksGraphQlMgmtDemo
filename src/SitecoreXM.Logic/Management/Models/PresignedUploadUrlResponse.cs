using Newtonsoft.Json;

namespace SitecoreXM.Logic.Management.ManagementApi
{
    public class PresignedUploadUrlResponse
    {
        [JsonProperty("uploadMedia")]
        public UploadMedia UploadMedia { get; set; }
    }
    public class UploadMedia
    {
        [JsonProperty("presignedUploadUrl")]
        public string PresignedUploadUrl { get; set; }
    }
}
