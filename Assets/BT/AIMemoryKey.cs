using UnityEngine;
using System.Collections;

public struct AIMemoryKey
{
    public enum ContextType
    {
        Unit,
        Squad,
        Global
    }

    public ContextType Context { get; private set; }
    public string Name { get; private set; }
    public int HashCode { get; private set; }

    public AIMemoryKey(string name)
        :this(name, ContextType.Unit)
    {
    }

    public AIMemoryKey(string name, AIMemoryKey.ContextType context)
    {
        Name = name;
        Context = context;
        HashCode = MemoryKeysHashCodeManager.RegisterMemoryKey(this);
    }
}
