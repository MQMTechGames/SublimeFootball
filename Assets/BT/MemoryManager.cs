using UnityEngine;

public class MemoryManager
{
    Memory _localMemory = new Memory();
    public Memory LocalMemory{ get{ return _localMemory; } set{ _localMemory = value; } }

    Memory _agentMemory = new Memory();
    public Memory AgentMemory{ get{ return _agentMemory; } set{ _agentMemory = value; } }
    
    Memory _sharedMemory;
    public Memory SharedMemory{ get{ return _sharedMemory; } set{ _sharedMemory = value; } }

    Memory _globalMemory;
    public Memory GlobalMemory{ get{ return _globalMemory; } set{ _globalMemory = value; } }

    public bool GetMemoryObject<T>(AIMemoryKey key, out T oVar)
    {
        if(key.Context == AIMemoryKey.ContextType.Local)
        {
            return LocalMemory.GetMemoryObject<T>(key, out oVar);
        }
        else if(key.Context == AIMemoryKey.ContextType.Agent)
        {
            return AgentMemory.GetMemoryObject<T>(key, out oVar);
        }
        else if(key.Context == AIMemoryKey.ContextType.Shared)
        {
            return SharedMemory.GetMemoryObject<T>(key, out oVar);
        }
        else
        {
            return GlobalMemory.GetMemoryObject<T>(key, out oVar);
        }
    }
    
    public void SetMemoryObject<T>(AIMemoryKey key, T iVar)
    {
        if(key.Context == AIMemoryKey.ContextType.Local)
        {
            LocalMemory.SetMemoryObject<T>(key, iVar);
        }
        else if(key.Context == AIMemoryKey.ContextType.Agent)
        {
            AgentMemory.SetMemoryObject<T>(key, iVar);
        }
        else if(key.Context == AIMemoryKey.ContextType.Shared)
        {
            SharedMemory.SetMemoryObject<T>(key, iVar);
        }
        else
        {
            GlobalMemory.SetMemoryObject<T>(key, iVar);
        }
    }
    
    public bool RemoveMemoryObject(AIMemoryKey key)
    {
        if(key.Context == AIMemoryKey.ContextType.Local)
        {
            return LocalMemory.RemoveMemoryObject(key);
        }
        else if(key.Context == AIMemoryKey.ContextType.Agent)
        {
            return AgentMemory.RemoveMemoryObject(key);
        }
        else if(key.Context == AIMemoryKey.ContextType.Shared)
        {
            return SharedMemory.RemoveMemoryObject(key);
        }
        else
        {
            return GlobalMemory.RemoveMemoryObject(key);
        }
    }
    
    public bool ContainsMemoryObject(AIMemoryKey key)
    {
        if(key.Context == AIMemoryKey.ContextType.Local)
        {
            return LocalMemory.ContainsMemoryObject(key);
        }
        else if(key.Context == AIMemoryKey.ContextType.Agent)
        {
            return AgentMemory.ContainsMemoryObject(key);
        }
        else if(key.Context == AIMemoryKey.ContextType.Shared)
        {
            return SharedMemory.ContainsMemoryObject(key);
        }
        else
        {
            return GlobalMemory.ContainsMemoryObject(key);
        }
    }
}
