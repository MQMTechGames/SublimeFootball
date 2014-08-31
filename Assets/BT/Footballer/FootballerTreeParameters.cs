using UnityEngine;

public class FootballerTreeParameters : TreeParameters
{
    public FootballerTreeParameters(BaseUnit unit, BaseSquad squad)
    {
        AddInputParameter(new AIMemoryKeyValue(UnitAIMemory.Unit, unit));
        AddInputParameter(new AIMemoryKeyValue(UnitAIMemory.Squad, squad));
    }
}
