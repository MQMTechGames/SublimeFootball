using UnityEngine;
using System.Collections.Generic;

public class SubTreeParameters
{
    [SerializeField]
    private List<AIMemoryKeyPair> _inputs;
    public List<AIMemoryKeyPair> Inputs
    {
        get
        { 
            if(_inputs == null)
            {
                _inputs = new List<AIMemoryKeyPair>();
            }
            
            return _inputs;
        }
    }
    
    [SerializeField]
    private List<AIMemoryKeyPair> _outputs;
    public List<AIMemoryKeyPair> Outputs
    {
        get
        { 
            if(_outputs == null)
            {
                _outputs = new List<AIMemoryKeyPair>();
            }
            
            return _outputs;
        }
    }

    public void AddInputParameter(AIMemoryKeyPair pair)
    {
        Inputs.Add(pair);
    }
    
    public void AddOutputParameter(AIMemoryKeyPair pair)
    {
        Outputs.Add(pair);
    }
}
