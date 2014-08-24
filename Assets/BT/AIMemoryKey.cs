public struct AIMemoryKey
{
    public enum ContextType
    {
        Local,
        Agent,
        Shared,
        Global
    }

    public ContextType Context { get; private set; }
    public string Name { get; private set; }
    public int HashCode { get; private set; }

    public AIMemoryKey(string name)
        :this(name, ContextType.Agent)
    {
    }

    public AIMemoryKey(string name, AIMemoryKey.ContextType context)
    {
        Name = name;
        Context = context;
        HashCode = MemoryKeysHashCodeManager.RegisterMemoryKey(this);
    }
}
