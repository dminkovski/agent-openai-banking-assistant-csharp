public class AgentContext: Dictionary<string, object>
{
    public AgentContext() : base()
    {
    }

    public AgentContext(string result) : base()
    {
        this["result"] = result;
    }

    public string Result
    {
        get => (string)this["result"];
        set => this["result"] = value;
    }

}

