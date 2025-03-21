namespace AgentBankingAssistant.Copilot.Interfaces
{
    public interface IToolsExecutionCache<T>
    {
        public void put();
        public T get();

        public List<T> values();

    }
}
