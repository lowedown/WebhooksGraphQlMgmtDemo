using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;
using SitecoreXM.Logic.Management.ManagementApi;
using SitecoreXM.Logic.Management.Models;

namespace SitecoreXM.Logic.Services
{
    public class ManagementApi
    {
        private readonly ILogger<ManagementApi> Logger;
        public readonly IConfiguration Configuration;

        public ManagementApi(ILogger<ManagementApi> logger, IConfiguration configuration) 
        {
            Logger = logger;
            Configuration = configuration;
        }        

        public GraphQLHttpClient GetClient()
        {
            var apiEndpoint = Configuration["Sitecore:ManagementApiUri"];            

            var graphQLHttpClientOptions = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri(apiEndpoint),
            };

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Configuration["Sitecore:ManagementApiToken"]}");

            return new GraphQLHttpClient(graphQLHttpClientOptions, new NewtonsoftJsonSerializer(), httpClient);
        }

        /// <summary>
        /// Uploads a file to the media library from an URL
        /// </summary>
        /// <param name="mediaPath">Relative path within media library i.e. Default Website/new media (don't include /sitecore/media libray)</param>
        /// <param name="fileUrl">Url to download the file.</param>
        /// <param name="fileEnding">Ending of the uploaded file i.E. png, jpg (without the .)</param>
        /// <returns></returns>
        public async Task<MediaUploadResult> UploadToMediaLibrary(string mediaPath, Uri fileUrl, string fileEnding)
        {
            var response = await new HttpClient().GetAsync(fileUrl);
            return await UploadToMediaLibrary(mediaPath, await response.Content.ReadAsStreamAsync(), fileEnding);
        }

        /// <summary>
        /// Uploads a file stream to the media library
        /// </summary>
        /// <param name="mediaPath">Relative path within media library i.e. Default Website/new media (don't include /sitecore/media libray)</param>
        /// <param name="fileData">Stream of the file to upload</param>
        /// <param name="fileEnding">Ending of the uploaded file i.E. png, jpg (without the .)</param>
        /// <returns></returns>
        public async Task<MediaUploadResult> UploadToMediaLibrary(string mediaPath, Stream fileData, string fileEnding)
        {
            var itemMutation = new GraphQLRequest
            {
                Query = @"
                mutation UploadImage($itemPath: String!)
                {
                  uploadMedia(input: { itemPath: $itemPath }) {
                    presignedUploadUrl
                  }
                }",
                OperationName = "UploadImage",
                Variables = new
                {
                    itemPath = mediaPath
                }
            };

            var graphQLResponse = await GetClient().SendQueryAsync<PresignedUploadUrlResponse>(itemMutation);

            string signedUploadUrl = graphQLResponse.Data.UploadMedia.PresignedUploadUrl;
            if (string.IsNullOrEmpty(signedUploadUrl))
            {
                Logger.LogError($"No valid presignedUploadUrl returned from Sitecore. Path: '{mediaPath}'");
                return new MediaUploadResult() { Success = false };
            }

            HttpContent fileStreamContent = new StreamContent(fileData);

            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(fileStreamContent, "file", $"file.{fileEnding}");

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Configuration["Sitecore:ManagementApiToken"]}");


                var uploadResponse = await httpClient.PostAsync(signedUploadUrl, formData);
                if (!uploadResponse.IsSuccessStatusCode)
                {
                    Logger.LogError($"Error uploading file to media library path '{mediaPath}'. Response: {uploadResponse.Content}");
                    return new MediaUploadResult() { Success = false };
                }
                
                var content = await uploadResponse.Content.ReadAsStringAsync();
                var apiUploadResult = JsonConvert.DeserializeObject<ApiMediaUploadResult>(content);
                if (apiUploadResult == null)
                {
                    Logger.LogError($"Error uploading file to media library path '{mediaPath}'. Response: {content}");
                    return new MediaUploadResult() { Success = false };
                }

                return new MediaUploadResult() { Success = true, MediaItemId = apiUploadResult.Id };
            }
        }
    }
}
