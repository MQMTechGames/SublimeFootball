using UnityEngine;
using System.Collections.Generic;

public class BaseSquad : MonoBehaviour
{
    [SerializeField]
    BaseUnit[] _units;

    [SerializeField]
    Goal _goal;

    [SerializeField]
    FieldSide _fieldSide;
    public FieldSide Side { get { return _fieldSide; } }

    Match _match;

    void Awake()
    {
        _units = gameObject.GetComponentsInChildren<BaseUnit>();
        foreach (BaseUnit unit in _units)
        {
            unit.Init(this);
        }

        _match = FindObjectOfType<Match>();
        DebugUtils.Assert(_match !=null, "_match !=null");
    }

    public BaseUnit GetClosestUnitToBall(Vector3 position)
    {
        DebugUtils.Assert(_units.Length > 0);

        float closestDist = float.MaxValue;
        BaseUnit closestUnit = null;

        foreach (BaseUnit unit in _units)
        {
            Vector3 unitToBallDir = unit.transform.position - position;
            float sqrMagnitude = unitToBallDir.sqrMagnitude;
            if(sqrMagnitude < closestDist)
            {
                closestDist = sqrMagnitude;
                closestUnit = unit;
            }
        }

        return closestUnit;
    }

    public BaseUnit SelectEasiestUnitToPassTheBall(BaseUnit passerUnit)
    {
        DebugUtils.Assert(_units.Length > 1);
        
        float closestDist = float.MaxValue;
        BaseUnit closestUnit = null;
        
        Vector3 passerPosition = passerUnit.transform.position;
        
        foreach (BaseUnit unit in _units)
        {
            if(unit == passerUnit)
            {
                continue;
            }

            Vector3 unitToBallDir = unit.transform.position - passerPosition;
            float sqrMagnitude = unitToBallDir.sqrMagnitude;
            if(sqrMagnitude < closestDist)
            {
                closestDist = sqrMagnitude;
                closestUnit = unit;
            }
        }
        
        return closestUnit;
    }

    public BaseUnit SelectForwardUnitToPassTheBall(BaseUnit passerUnit, Vector3 forwardDirection, bool discartItself)
    {
        DebugUtils.Assert(_units.Length > 1, "no mates to pass the ball :(");
        if(_units.Length < 1)
        {
            return null;
        }
        
        float closestDist = float.MinValue;
        BaseUnit closestUnit = null;
        
        Vector3 passerPosition = passerUnit.transform.position;
        
        foreach (BaseUnit unit in _units)
        {
            if(unit == passerUnit && discartItself)
            {
                continue;
            }
            
            float dotResult = Vector3.Dot(unit.transform.position, forwardDirection);
            if(dotResult > closestDist)
            {
                closestDist = dotResult;
                closestUnit = unit;
            }
        }
        
        return closestUnit;
    }

    public BaseUnit SelectRandomUnitToPassTheBall(BaseUnit passerUnit)
    {
        DebugUtils.Assert(_units.Length > 1, "no mates to pass the ball :(");
        if(_units.Length < 1)
        {
            return null;
        }

        BaseUnit selected = null;
        while(selected == null || selected == passerUnit)
        {
            int idx = Random.Range(0, _units.Length);
            selected = _units[idx];
        }

        return selected;
    }

    public Goal FindGoal()
    {
        return _goal;
    }
}
