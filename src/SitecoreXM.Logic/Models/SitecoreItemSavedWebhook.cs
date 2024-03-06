namespace SitecoreXM.Logic.Models
{
    public class SitecoreItemSavedWebhook
    {
        public string EventName { get; set; }
        public Item Item { get; set; }
        public Changes Changes { get; set; }
        public string WebhookItemId { get; set; }
        public string WebhookItemName { get; set; }
    }

    public class Changes
    {
        public List<FieldChange> FieldChanges { get; set; }
        public List<object> PropertyChanges { get; set; }
        public bool IsUnversionedFieldChanged { get; set; }
        public bool IsSharedFieldChanged { get; set; }
    }

    public class FieldChange
    {
        public Guid FieldId { get; set; }
        public string Value { get; set; }
        public string OriginalValue { get; set; }
    }

    public class Item
    {
        public string Language { get; set; }
        public int Version { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public Guid TemplateId { get; set; }
        public Guid MasterId { get; set; }
        public List<SharedField> SharedFields { get; set; }
        public List<UnversionedField> UnversionedFields { get; set; }
        public List<VersionedField> VersionedFields { get; set; }
    }

    public class SharedField
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
    }

    public class UnversionedField
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public string Language { get; set; }
    }

    public class VersionedField
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public int Version { get; set; }
        public string Language { get; set; }
    }
}
