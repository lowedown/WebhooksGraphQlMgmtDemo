namespace SitecoreXM.Logic.Models
{
    public class OpenAIImageResponse
    {
        public int created { get; set; }
        public List<Data> data { get; set; }
    }

    public class Data
    {
        public string revised_prompt { get; set; }
        public string url { get; set; }
    }
}
