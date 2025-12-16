namespace DecisionCore.ValueObjects
{
    public record DecisionText
    {
        public string Arabic { get; }
        public string English { get; }

        private DecisionText(string arabic, string english)
        {
            Arabic = arabic;
            English = english;
        }

        public static DecisionText Of(string arabic, string english)
        {
            if (string.IsNullOrWhiteSpace(arabic))
                throw new ArgumentException("Arabic text is required.");

            if (string.IsNullOrWhiteSpace(english))
                throw new ArgumentException("English text is required.");

            return new DecisionText(arabic, english);
        }
    }
}
