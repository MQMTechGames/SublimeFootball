using UnityEngine;

public class MemoryManager
{
    Memory _unitMemory = new Memory();
    public Memory UnitMemory{ get{ return _unitMemory; } set{ _unitMemory = value; } }
    
    Memory _squadMemory;
    public Memory SquadMemory{ get{ return _squadMemory; } set{ _squadMemory = value; } }

    Memory _globalMemory;
    public Memory GlobalMemory{ get{ return _globalMemory; } set{ _globalMemory = value; } }

    public bool GetMemoryObject<T>(AIMemoryKey key, out T oVar)
    {
        if(key.Context == AIMemoryKey.ContextType.Unit)
        {
            return UnitMemory.GetMemoryObject<T>(key.Name, out oVar);
        }
        else if(key.Context == AIMemoryKey.ContextType.Squad)
        {
            return SquadMemory.GetMemoryObject<T>(key.Name, out oVar);
        }
        else
        {
            return GlobalMemory.GetMemoryObject<T>(key.Name, out oVar);
        }
    }
    
    public void SetMemoryObject<T>(AIMemoryKey key, T iVar)
    {
        if(key.Context == AIMemoryKey.ContextType.Unit)
        {
            UnitMemory.SetMemoryObject<T>(key.Name, iVar);
        }
        else if(key.Context == AIMemoryKey.ContextType.Squad)
        {
            SquadMemory.SetMemoryObject<T>(key.Name, iVar);
        }
        else
        {
            GlobalMemory.SetMemoryObject<T>(key.Name, iVar);
        }
    }
    
    public bool RemoveMemoryObject(AIMemoryKey key)
    {
        if(key.Context == AIMemoryKey.ContextType.Unit)
        {
            return UnitMemory.RemoveMemoryObject(key.Name);
        }
        else if(key.Context == AIMemoryKey.ContextType.Squad)
        {
            return SquadMemory.RemoveMemoryObject(key.Name);
        }
        else
        {
            return GlobalMemory.RemoveMemoryObject(key.Name);
        }
    }
    
    public bool ContainsMemoryObject(AIMemoryKey key)
    {
        if(key.Context == AIMemoryKey.ContextType.Unit)
        {
            return UnitMemory.ContainsMemoryObject(key.Name);
        }
        else if(key.Context == AIMemoryKey.ContextType.Squad)
        {
            return SquadMemory.ContainsMemoryObject(key.Name);
        }
        else
        {
            return GlobalMemory.ContainsMemoryObject(key.Name);
        }
    }
}
