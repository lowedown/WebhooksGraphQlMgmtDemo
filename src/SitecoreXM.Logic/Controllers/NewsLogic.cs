using GraphQL;
using Microsoft.AspNetCore.Mvc;
using SitecoreXM.Logic.Models;
using SitecoreXM.Logic.OpenAI;
using SitecoreXM.Logic.Services;
using System.Text.RegularExpressions;

namespace SitecoreXM.Logic.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewsLogic : ControllerBase
    {
        private readonly ILogger Logger;
        private readonly ManagementApi ManagementApi;
        private readonly IConfiguration Configuration;
        private readonly OpenAIService openAIService;

        public NewsLogic(ILogger<NewsLogic> logger, ManagementApi managementApi, IConfiguration configuration, OpenAIService openAIService)
        {
            Logger = logger;
            ManagementApi = managementApi;
            this.Configuration = configuration;
            this.openAIService = openAIService;
        }

        [HttpPost("ValidateIdeation")]
        public ValidationReponse Validate([FromBody] SitecoreWorkflowValidationWebhook validationEvent)
        {
            // NOTE: Timeout is very short on validation events!

            var authorField = validationEvent.DataItem.SharedFields.FirstOrDefault(field => field.Id == new Guid("{ca8b6251-c526-420b-be32-d9604fca1cee}"));

            if (authorField == null || string.IsNullOrWhiteSpace(authorField.Value)) {
                return new ValidationReponse()
                {
                    IsValid = false,
                    Message = "Please enter an author."
                };
            }

            return new ValidationReponse()
            {
                IsValid = true
            };
        }

        [HttpPost("RunGenAI")]
        public async void RunGenAI([FromBody] SitecoreWorkflowActionWebhook workflowAction)
        {
            // Let's generate a prompt using the first comment
            var prompt = $"Generate a news article based on the following topic: {workflowAction.Comments[0].Value}";
            var openAiArticle = await this.openAIService.ChatCompletion(prompt);  
            
            if (openAiArticle == null)
            {
                throw new Exception("Unable to generate text from OpenAI");
            }
                      
            // Update the news article
            var itemMutation = new GraphQLRequest
            {
                Query = @"
                mutation ModifyItem($database: String, $itemId: ID, $itemLanguage: String, $content:String, $contentAbstract:String, $workflowState: ID){
                    updateItem(
                        input: {
                            database: $database
                            itemId: $itemId
                            language: $itemLanguage
                            fields: [
                                { name: ""Article Content"", value: $content },
                                { name: ""Abstract"", value: $contentAbstract },
                                { name: ""__Workflow state"", value: $workflowState }
                            ]
                        }
                    ) {
                    item {
                      itemId
                    }
                  }
                }",
                OperationName = "ModifyItem",
                Variables = new
                {
                    database = "master", 
                    itemId = workflowAction.DataItem.Id,
                    itemLanguage = workflowAction.DataItem.Language,
                    content = openAiArticle,
                    contentAbstract = workflowAction.Comments[0].Value,
                    workflowState = Guid.Parse("{B18B5193-F836-417A-BD40-9DBEBB2A40DF}") // Manually transition on to next workflow state
                }
            };

            var graphQLResponse = await ManagementApi.GetClient().SendQueryAsync<object>(itemMutation);
        }

        [HttpPost("GenerateImage")]
        public async void GenerateImage([FromBody] SitecoreWorkflowActionWebhook workflowAction)
        {
            var imageUrl = await openAIService.GenerateImage(workflowAction.Comments[0].Value);

            if (imageUrl == null)
            {
                throw new Exception("Error generating image through OpenAI");
            }

            // var imageUrl = "https://oaidalleapiprodscus.blob.core.windows.net/private/org-v9eTDNoJ8clpDsb96HIzgSIU/user-HBhLcd4LBXO7rAGtArVWMebr/img-GFxZDb5gM2XpAQ2onsBqpyWq.png?st=2024-03-03T21%3A11%3A09Z&se=2024-03-03T23%3A11%3A09Z&sp=r&sv=2021-08-06&sr=b&rscd=inline&rsct=image/png&skoid=6aaadede-4fb3-4698-a8f6-684d7786b067&sktid=a48cca56-e6da-484e-a814-9c849652bcb3&skt=2024-03-03T17%3A27%3A46Z&ske=2024-03-04T17%3A27%3A46Z&sks=b&skv=2021-08-06&sig=Pp8MHLqs3BfnAWKb2A6JRi7/p4YH3bA%2BcopKJWptom0%3D";

            // Create a unique media item name
            var mediaItemName = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");

            var result = await ManagementApi.UploadToMediaLibrary($"Default Website/news/{mediaItemName}", imageUrl, "png");

            if (result == null || result.Success == false) 
            {
                throw new Exception("Unable to upload media.");
            }

            var itemMutation = new GraphQLRequest
            {
                Query = @"
                mutation ModifyItem($database: String, $itemId: ID, $itemLanguage: String, $mediaItemLink:String, $imageDescription:String, $workflowState: ID){
                    updateItem(
                        input: {
                            database: $database
                            itemId: $itemId
                            language: $itemLanguage
                            fields: [
                                { name: ""Title Image"", value: $mediaItemLink },
                                { name: ""Title Image Description"", value: $imageDescription },
                                { name: ""__Workflow state"", value: $workflowState }
                            ]
                        }
                    ) {
                    item {
                      itemId
                    }
                  }
                }",
                OperationName = "ModifyItem",
                Variables = new
                {
                    database = "master",
                    itemId = workflowAction.DataItem.Id,
                    itemLanguage = workflowAction.DataItem.Language,
                    mediaItemLink = $"<image mediaid=\"{result.MediaItemId}\" alt=\"{workflowAction.Comments[0].Value}\" />",
                    imageDescription = workflowAction.Comments[0].Value,
                    workflowState = Guid.Parse("{2F78E38F-6870-409E-9845-9597B697479B}") // Manually transition on to next workflow state
                }
            };

            var graphQLResponse = await ManagementApi.GetClient().SendQueryAsync<object>(itemMutation);
        }
    }
}
