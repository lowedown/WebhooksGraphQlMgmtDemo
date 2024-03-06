using GraphQL;
using Microsoft.AspNetCore.Mvc;
using SitecoreXM.Logic.Management.Models;
using SitecoreXM.Logic.Models;
using SitecoreXM.Logic.Services;

namespace SitecoreXM.Logic.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GraphQLDemo : ControllerBase
    {
        private readonly ILogger Logger;
        private readonly ManagementApi ManagementApi;
        
        public GraphQLDemo(ILogger<Webhooks> logger, ManagementApi managementApi)
        {
            Logger = logger;
            ManagementApi = managementApi;
        }

        [HttpPost("ItemSaved")]
        public async Task<string> Saved([FromBody] SitecoreItemSavedWebhook savedEvent)
        {
            // Prevent infinite saving-loop
            if (GetModifierUser(savedEvent) == @"sitecore\GraphqlAPI")
            {
                Logger.LogDebug("Ignored save triggered by ApiUser to prevent infinite loop.");
                return "";
            }

            var titleFieldId = new Guid("{75577384-3C97-45DA-A847-81B00500E250}");
            var titleField = savedEvent.Changes.FieldChanges.FirstOrDefault(change => change.FieldId == titleFieldId);            
            if (titleField == null)
            {
                Logger.LogDebug("Title field was not modified. Ignoring event.");
                return "";
            }

            // Update the news article
            var itemMutation = new GraphQLRequest
            {
                Query = @"
                mutation ModifyItem($database: String, $itemId: ID, $itemLanguage: String, $title:String){
                    updateItem(
                        input: {
                            database: $database
                            itemId: $itemId
                            language: $itemLanguage
                            fields: [
                                   { name: ""title"", value: $title }
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
                    itemId = savedEvent.Item.Id.ToString(),
                    itemLanguage = savedEvent.Item.Language,
                    title = titleField.Value.Replace("$currentDate", DateTime.Now.ToShortDateString()),
                }
            };

            var graphQLResponse = await ManagementApi.GetClient().SendQueryAsync<UpdateItemResponse>(itemMutation);

            return $"Updated: ${graphQLResponse.Data.UpdateItem.Item.ItemId}";
        }

        private static string GetModifierUser(SitecoreItemSavedWebhook value)
        {
            Guid LastUpdatedFieldId = new("{badd9cf9-53e0-4d0c-bcc0-2d784c282f6a}");
            return value.Changes.FieldChanges.First(change => change.FieldId == LastUpdatedFieldId).Value;
        }
    }
}
