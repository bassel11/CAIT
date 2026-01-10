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
                throw new DomainException("Arabic text is required for the decision.");

            if (string.IsNullOrWhiteSpace(english))
                throw new DomainException("English text is required for the decision.");

            return new DecisionText(arabic, english);
        }
    }
}
