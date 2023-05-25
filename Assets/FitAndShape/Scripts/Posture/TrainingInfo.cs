namespace FitAndShape
{
    public struct TrainingInfo
    {
        public string name { get; set; }
        public string description { get; set; }

        public TrainingInfo(string name, string description)
        {
            this.name = name;
            this.description = description;
        }
    }
}
