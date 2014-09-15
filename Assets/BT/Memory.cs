using UnityEngine;
using System.Collections.Generic;

public class Memory
{
    Dictionary<AIMemoryKey, System.Object> _map = new Dictionary<AIMemoryKey, System.Object>();
    public Dictionary<AIMemoryKey, System.Object> Map { get{ return _map; } }

    public bool GetMemoryObject<T>(AIMemoryKey name, out T oVar)
    {
        oVar = default(T);
        System.Object obj;
        bool isOk = _map.TryGetValue(name, out obj);
        isOk &= obj is T;
        
        if(isOk)
        {
            oVar = (T) obj;
        }
        
        return isOk;
    }
    
    public void SetMemoryObject<T>(AIMemoryKey name, T oVar)
    {
        if (ContainsMemoryObject(name)) 
        {
            RemoveMemoryObject(name);
        }
        
        _map.Add(name, oVar);
    }
    
    public bool RemoveMemoryObject(AIMemoryKey name)
    {
        if (ContainsMemoryObject(name)) 
        {
            _map.Remove(name);
            
            return true;
        }
        
        return false;
    }
    
    public bool ContainsMemoryObject(AIMemoryKey name)
    {
        return _map.ContainsKey(name);
    }
}
