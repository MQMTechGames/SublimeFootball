using UnityEngine;
using System.Collections.Generic;

public class BaseSquad : MonoBehaviour
{
    [SerializeField]
    BaseUnit[] _units;

    [SerializeField] // Serialized temporally
    bool _havePosession;

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

    public void OnBallLost()
    {
        _havePosession = false;
    }

    public void OnBallControlled()
    {
        _havePosession = true;
    }

    public bool HaveSquadPosession()
    {
        return _havePosession;
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

    public BaseUnit GetEasiestUnitToPassTheBall(BaseUnit passerUnit)
    {
        DebugUtils.Assert(_units.Length > 0);
        
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
}
