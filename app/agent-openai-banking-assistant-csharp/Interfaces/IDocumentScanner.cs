namespace agent_openai_banking_assistant_csharp.Interfaces
{
    public interface IDocumentScanner
    {
        public Dictionary<string, string> Scan(String fileName);
    }
}
