using System.Collections.Generic;
using UnityEngine;

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
        HashCode = Animator.StringToHash(Name); //  Name.GetHashCode();
    }

    public class EqualityComparer : IEqualityComparer<AIMemoryKey>
    {   
        public bool Equals(AIMemoryKey x, AIMemoryKey y)
        {
            return x.HashCode == y.HashCode;
        }
        
        public int GetHashCode(AIMemoryKey x)
        {
            return x.GetHashCode();
        }
    }
}
