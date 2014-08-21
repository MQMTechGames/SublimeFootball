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
            return UnitMemory.GetMemoryObject<T>(key.HashCode, out oVar);
        }
        else if(key.Context == AIMemoryKey.ContextType.Squad)
        {
            return SquadMemory.GetMemoryObject<T>(key.HashCode, out oVar);
        }
        else
        {
            return GlobalMemory.GetMemoryObject<T>(key.HashCode, out oVar);
        }
    }
    
    public void SetMemoryObject<T>(AIMemoryKey key, T iVar)
    {
        if(key.Context == AIMemoryKey.ContextType.Unit)
        {
            UnitMemory.SetMemoryObject<T>(key.HashCode, iVar);
        }
        else if(key.Context == AIMemoryKey.ContextType.Squad)
        {
            SquadMemory.SetMemoryObject<T>(key.HashCode, iVar);
        }
        else
        {
            GlobalMemory.SetMemoryObject<T>(key.HashCode, iVar);
        }
    }
    
    public bool RemoveMemoryObject(AIMemoryKey key)
    {
        if(key.Context == AIMemoryKey.ContextType.Unit)
        {
            return UnitMemory.RemoveMemoryObject(key.HashCode);
        }
        else if(key.Context == AIMemoryKey.ContextType.Squad)
        {
            return SquadMemory.RemoveMemoryObject(key.HashCode);
        }
        else
        {
            return GlobalMemory.RemoveMemoryObject(key.HashCode);
        }
    }
    
    public bool ContainsMemoryObject(AIMemoryKey key)
    {
        if(key.Context == AIMemoryKey.ContextType.Unit)
        {
            return UnitMemory.ContainsMemoryObject(key.HashCode);
        }
        else if(key.Context == AIMemoryKey.ContextType.Squad)
        {
            return SquadMemory.ContainsMemoryObject(key.HashCode);
        }
        else
        {
            return GlobalMemory.ContainsMemoryObject(key.HashCode);
        }
    }
}
