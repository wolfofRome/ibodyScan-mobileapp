namespace FitAndShape
{
    public class Description
    {
        public string title { get; set; }
        public string summary { get; set; }
        public string detail { get; set; }
        public string advice { get; set; }
        public Description(string title, string summary, string detail, string advice)
        {
            this.title = title;
            this.summary = summary;
            this.detail = detail;
            this.advice = advice;
        }
    }
}
