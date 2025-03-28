public class IntentResponse
    {
        public IntentType intentType { get; set; }
        public string message { get; set; }

        public IntentResponse(IntentType intentType, string message)
        {
            this.intentType = intentType;
            this.message = message;
        }
    }
