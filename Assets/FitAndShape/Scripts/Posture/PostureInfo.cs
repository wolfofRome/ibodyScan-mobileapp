namespace FitAndShape
{
    public struct PostureInfo
    {
        public RangeThreshold threshold { get; set; }
        public string description { get; set; }
        public BonePart[] boneParts { get; set; }

        public PostureInfo(RangeThreshold threshold, string description, BonePart[] boneParts)
        {
            this.threshold = threshold;
            this.description = description;
            this.boneParts = boneParts;
        }
    }
}
