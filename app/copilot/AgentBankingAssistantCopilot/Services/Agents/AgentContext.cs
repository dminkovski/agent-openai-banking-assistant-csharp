namespace AgentBankingAssistant.Copilot.Services.Agents
{
    public class AgentContext: Dictionary<string, object>
    {
        private string result = "";

        public AgentContext()
        {

        }
        public AgentContext(string result)
        {
            this["result"] = result;
        }

        public string getResult()
        {
            return result;
        }

        public void setResult(string result)
        {
            this["result"] = result;
        }
    }
}
