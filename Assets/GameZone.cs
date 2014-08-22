using UnityEngine;
using System.Collections;

public class GameZone : MonoBehaviour
{
    [SerializeField]
    GameZoneType _zoneType;

    public GameZoneType ZoneType 
    {
        get{ return _zoneType; }
        set{ _zoneType = value; }
    }
}
