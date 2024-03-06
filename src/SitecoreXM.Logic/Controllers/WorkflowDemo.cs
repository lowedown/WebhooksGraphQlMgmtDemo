using Microsoft.AspNetCore.Mvc;
using SitecoreXM.Logic.Models;
using SitecoreXM.Logic.Services;
using System.Text.Json.Nodes;

namespace SitecoreXM.Logic.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkflowDemo : ControllerBase
    {
        [HttpPost("Validate")]
        public ValidationReponse Validate([FromBody] SitecoreWorkflowValidationWebhook validationEvent)
        {
            // NOTE: Timeout is very short on validation events!

            var titleField = validationEvent.DataItem.VersionedFields.FirstOrDefault(field => field.Id == new Guid("{1376F066-01A8-49EE-8C63-2091EE9E1355}"));

            if (titleField == null || string.IsNullOrWhiteSpace(titleField.Value))
            {
                return new ValidationReponse()
                {
                    IsValid = false,
                    Message = "Please enter a title."
                };
            }


            var textField = validationEvent.DataItem.VersionedFields.FirstOrDefault(field => field.Id == new Guid("{E4DBA393-B056-4BC0-8360-59ED2D96841F}"));
            if (textField == null || string.IsNullOrWhiteSpace(textField.Value))
            {
                return new ValidationReponse()
                {
                    IsValid = false,
                    Message = "Please enter a text."
                };
            }


            return new ValidationReponse()
            {
                IsValid = true
            };
        }

        [HttpPost("Submit")]
        public async void Submit([FromBody] SitecoreWorkflowActionWebhook workflowAction)
        {
            // Do something on submit
        }
    }
}
