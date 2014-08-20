using UnityEngine;
using System.Collections.Generic;

public class Memory
{
    Dictionary<string, System.Object> _map = new Dictionary<string, object>();
    public Dictionary<string, System.Object> Map { get{ return _map; } }

    public bool GetMemoryObject<T>(string name, out T oVar)
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
    
    public void SetMemoryObject<T>(string name, T oVar)
    {
        if (ContainsMemoryObject(name)) 
        {
            RemoveMemoryObject(name);
        }
        
        _map.Add(name, oVar);
    }
    
    public bool RemoveMemoryObject(string name)
    {
        if (ContainsMemoryObject(name)) 
        {
            _map.Remove(name);
            
            return true;
        }
        
        return false;
    }
    
    public bool ContainsMemoryObject(string name)
    {
        return _map.ContainsKey(name);
    }
}
