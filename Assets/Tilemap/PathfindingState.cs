using UnityEngine;
using System.Collections.Generic;

public class PathfindingState : MonoBehaviour
{
    Dictionary<Vector2, bool> _visitedNodes = new Dictionary<Vector2, bool>();
    public Dictionary<Vector2, bool> VisitedNodes { get { return _visitedNodes; } }

    public Vector2 CurrentTileId { get; set; }

    public PathfindingState(Vector2 currentTileId)
    {
        CurrentTileId = currentTileId;
    }

    public PathfindingState() 
    { }

    public PathfindingState Clone()
    {
        PathfindingState state = new PathfindingState();

        // Copy CurrentTileId
        state.CurrentTileId = CurrentTileId;

        // Copy Visited Nodes
        foreach (var item in _visitedNodes)
        {
            state.VisitedNodes.Add(item.Key, item.Value); 
        }

        return state;
    }
}
