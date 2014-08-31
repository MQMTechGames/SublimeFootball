using UnityEngine;
using System.Collections.Generic;

public class TreeParameters
{
    [SerializeField]
    private List<AIMemoryKeyValue> _inputs;
    public List<AIMemoryKeyValue> Inputs
    {
        get
        { 
            if(_inputs == null)
            {
                _inputs = new List<AIMemoryKeyValue>();
            }
            
            return _inputs;
        }
    }

    public void AddInputParameter(AIMemoryKeyValue keyValuePair)
    {
        Inputs.Add(keyValuePair);
    }
}
