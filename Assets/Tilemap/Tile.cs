using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    #region PathfindingData
    public bool IsOpen { get; set; }
    public bool IsClosed { get; set; }
    public Tile Parent { get; set; }

    public bool IsWalkable = true;
    public bool IsInPath = false;
    public bool IsJumped = false;

    public float H { get; set; }
    public float G { get; set; }
    public float F 
    {
        get
        {
            return H + G;
        }
    }
    #endregion PathfindingData

    Tilemap _tilemap;
    public Tilemap Map
    {
        get 
        { 
            if (_tilemap == null)
            {
                _tilemap = FindObjectOfType<Tilemap>();
                DebugUtils.Assert(_tilemap!=null, "_tilemap!=null");
            }
            
            return _tilemap;
        } 
    }

    Vector2 _id;
    public Vector2 Id 
    {
        get { return _id; }
    }

    Vector3 _worldPos;
    public Vector3 WorldPos
    {
        get { return _worldPos; }
    }

    public Color DebugColor = Color.blue;

    public Tile ()
    {
        Parent = null;
        IsOpen = false;
        IsClosed = false;
        IsInPath = false;
        IsJumped = false;
        G = 0f;
        H = 0f;
    }

    public void ResetPathfindingValues()
    {
        renderer.material.color = Color.white;
        Parent = null;
        IsOpen = false;
        IsClosed = false;
        IsInPath = false;
        IsJumped = false;
        G = 0f;
        H = 0f;
    }

    public void Init(Vector2 id, Vector3 position)
    {
        _id = id;
        transform.position = position;
        _worldPos = position;
    }

    void Update()
    {
        if(IsJumped)
        {
            renderer.material.color = Color.cyan;
        }
        if(IsOpen)
        {
            renderer.material.color = Color.gray;
        }
        if(IsClosed)
        {
            renderer.material.color = Color.black;
        }
        if(IsInPath)
        {
            renderer.material.color = Color.yellow;
        }
        if(!IsWalkable)
        {
            renderer.material.color = new Color(20, 0, 10);
        }
    }

    public void DoOnDrawGizmos()
    {
        Gizmos.color = Color.white;

        if(IsJumped)
        {
            Gizmos.color = Color.cyan;
        }
        if(IsOpen)
        {
            Gizmos.color = Color.gray;
        }
        if(IsClosed)
        {
            Gizmos.color = Color.black;
        }
        if(IsInPath)
        {
            Gizmos.color = Color.yellow;
        }
        if(!IsWalkable)
        {
            Gizmos.color = new Color(20, 0, 10);
        }

        Gizmos.DrawCube(transform.position, new Vector3(0.9f, 0.9f, 0.9f));
    }

    public void OnDrawGizmosSelected()
    {
        List<Vector2> tiles = Map.GetNeighbors(Id);

        for (int i = 0; i < tiles.Count; ++i)
        {
            Tile tile = Map.GetTileByTileId(tiles[i]);
            Gizmos.color = Color.red;
            Gizmos.DrawCube(tile.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
        }
    }

    void OnMouseUp()
    {
        IsWalkable = !IsWalkable;

        if(IsWalkable)
        {
            renderer.material.color = Color.white;
        }
        else
        {
            renderer.material.color = new Color(20, 0, 10);
        }
    }
}
