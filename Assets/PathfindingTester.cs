using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof (Pathfinding))]
public class PathfindingTester : MonoBehaviour
{
    private Pathfinding _pathfinding;
    private List<Vector3> _path;

    public Vector3 StartWorldPos;
    public Vector3 EndWorldPos;

    public bool Test;

    void Awake()
    {
        _pathfinding = GetComponent<Pathfinding>();
    }

    void Update()
    {
        if(Test)
        {
            Test = false;

            _path = _pathfinding.FindPath(StartWorldPos, EndWorldPos);
        }
//        if(_path != null && _path.Count > 0)
//        {
//            Debug.Log("Path Count : " + _path.Count);
//        }
    }

    void OnDrawGizmosSelected()
    {
        if(_path == null)
        {
            return;
        }

        for (int i = 0; i < _path.Count; ++i)
        {
            Gizmos.color = Color.yellow;

            Vector3 worldPos = _path[i];
            Gizmos.DrawCube(worldPos, new Vector3(1, 1, 1));
        }
    }
}
