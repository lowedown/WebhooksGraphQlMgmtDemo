namespace SitecoreXM.Logic.Models
{
    public class SitecoreWorkflowValidationWebhook
    {
        public Guid ActionID { get; set; }
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

    public class Comment
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class DataItem
    {
        public string Language { get; set; }
        public int Version { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ParentId { get; set; }
        public Guid TemplateId { get; set; }
        public Guid MasterId { get; set; }
        public List<SharedField> SharedFields { get; set; }
        public List<object> UnversionedFields { get; set; }
        public List<VersionedField> VersionedFields { get; set; }
    }

    public class NextState
    {
        public string DisplayName { get; set; }
        public bool FinalState { get; set; }
        public string Icon { get; set; }
        public string StateID { get; set; }
        public List<object> PreviewPublishingTargets { get; set; }
    }

    public class PreviousState
    {
        public string DisplayName { get; set; }
        public bool FinalState { get; set; }
        public string Icon { get; set; }
        public string StateID { get; set; }
        public List<object> PreviewPublishingTargets { get; set; }
    }
}
