namespace SitecoreXM.Logic.Models
{
    public class SitecoreWorkflowActionWebhook
    {
        public string ActionID { get; set; }
        public string ActionName { get; set; }
        public List<Comment> Comments { get; set; }
        public DataItem DataItem { get; set; }
        public string Message { get; set; }
        public NextState? NextState { get; set; }
        public PreviousState? PreviousState { get; set; }
        public string UserName { get; set; }
        public string WorkflowName { get; set; }
        public string WebhookItemId { get; set; }
        public string WebhookItemName { get; set; }
    }
}
