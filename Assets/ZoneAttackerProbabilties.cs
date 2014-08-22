using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ZoneProbability
{
    public GameZoneType ZoneType;
    public float ShootProbability;
    public float PassProbability;
}

public class ZoneAttackerProbabilties : MonoBehaviour
{
    [SerializeField]
    List<ZoneProbability> _zones;
}
