using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Tilemap))]
public class Pathfinding : MonoBehaviour
{
    Tilemap _map;

    Tile _start;
    Tile _end;

    List<Tile> _openList = new List<Tile>();
    List<Vector3> _path = new List<Vector3>();

    void Awake()
    {
        _map = GetComponent<Tilemap>();
    }

    public List<Vector3> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
    {
        _openList.Clear();
        _map.ResetPathfindingValues();

        _path.Clear();

        _start = _map.GetTileByWorldPos(startWorldPos);
        DebugUtils.Assert(_start!=null, "_start!=null");

        _end = _map.GetTileByWorldPos(endWorldPos);
        DebugUtils.Assert(_end!=null, "_end!=null");

        _openList.Add(_start);

        while(!FindPathStep())
        {
        }

        return _path;
    }

    bool FindPathStep()
    {
        if(_openList.Count <= 0)
        {
            return true;
        }

        Tile current = _openList[0];
        current.IsClosed = true;
        _openList.Remove(current);

        if(current == _end)
        {
            BuildPath(current);
            return true;
        }

        FindSuccessors(current);

        return false;
    }

    void FindSuccessors(Tile current)
    {
        List<Vector2> neighbors = new List<Vector2>();
        //_map.GetNeighborsIds(current.Id, neighbors);
        GetNeighborsIds(current, current.Parent, neighbors);
        
        for (int i = 0; i < neighbors.Count; ++i)
        {
            Vector2 neighborgId = neighbors[i];
            //Tile neighborg = _map.GetTileByTileId(neighborgId);
            Tile neighborg = Jump((int) neighborgId.x, (int) neighborgId.y, (int) current.Id.x, (int) current.Id.y);

            if(   neighborg == null
               || neighborg.IsClosed
               || !neighborg.IsWalkable
               )
            {
                continue;
            }

            float g = current.G + (current.Id - neighborgId).sqrMagnitude;
            float h = (_end.Id - neighborgId).sqrMagnitude;

            AddToOpenList(neighborg, current, g, h);
        }
    }

    Tile Jump(int x, int y, int px, int py)
    {
        Tile current = _map.GetTileByTileId(new Vector2(x, y));
        if(current == null || !current.IsWalkable)
        {
            return null;
        }

        current.IsJumped = true;

        if(current == _end)
        {
            return current;
        }

        int dx = (x - px) / Mathf.Max(Mathf.Abs((x - px)), 1);
        int dy = (y - py) / Mathf.Max(Mathf.Abs((y - py)), 1);

        // Diagonal movement
        if(dx != 0 && dy != 0)
        {
            if(IsWalkableAt(x + dx, y + dy) && (!IsWalkableAt(x, y - dy) || !IsWalkableAt(x - dx, y)))
            {
                return current;
            }

            if(Jump(x + dx, y, x, y) != null || Jump(x, y + dy, x, y) != null)
            {
                return current;
            }
        }
        // Horizontal movement
        else
        {
            if(dx != 0)
            {
                if (  (!IsWalkableAt(x, y +1) && IsWalkableAt(x + dx, y +1))
                   || (!IsWalkableAt(x, y -1) && IsWalkableAt(x + dx, y -1))
                   )
                {
                    return current;
                }
            }
            else
            {
                if (   (!IsWalkableAt(x +1, y) && IsWalkableAt(x +1, y +dy))
                    || (!IsWalkableAt(x -1, y) && IsWalkableAt(x -1, y +dy))
                   )
                {
                    return current;
                }
            }
        }

        return Jump(x + dx, y + dy, x, y);
    }

    void GetNeighborsIds(Tile current, Tile parent, List<Vector2> neighbors)
    {
        if(parent != null)
        {
            int x = (int) current.Id.x;
            int y = (int) current.Id.y;

            int px = (int) parent.Id.x;
            int py = (int) parent.Id.y;

            int dx = (x - px) / Mathf.Max(Mathf.Abs((x - px)), 1);
            int dy = (y - py) / Mathf.Max(Mathf.Abs((y - py)), 1);

            // Diagonal
            if(dx != 0 && dy != 0)
            {
                // basic neighborgs
                neighbors.Add(new Vector2(x + dx, y + dy));
                neighbors.Add(new Vector2(x + dx, y));
                neighbors.Add(new Vector2(x, y + dy));

                // forced neighborgs
                if(!IsWalkableAt(x, y -dy))
                {
                    neighbors.Add(new Vector2(x +dx, y -dy));
                }

                if(!IsWalkableAt(x -dx, y))
                {
                    neighbors.Add(new Vector2(x -dx, y +dy));
                }
            }

            // Horizontal
            else
            {
                if(dx != 0)
                {
                    neighbors.Add(new Vector2(x + dx, y));

                    if(!IsWalkableAt(x, y +1))
                    {
                        neighbors.Add(new Vector2(x +dx, y +1));
                    }
                    
                    if(!IsWalkableAt(x, y -1))
                    {
                        neighbors.Add(new Vector2(x +dx, y -1));
                    }
                }
                else
                {
                    neighbors.Add(new Vector2(x, y + dy));
                    
                    if(!IsWalkableAt(x +1, y))
                    {
                        neighbors.Add(new Vector2(x +1, y +dy));
                    }
                    
                    if(!IsWalkableAt(x -1, y))
                    {
                        neighbors.Add(new Vector2(x -1, y +dy));
                    }
                }
            }
        }
        else
        {
            _map.GetNeighborsIds(current.Id, neighbors);
        }
    }

    bool IsWalkableAt(int x, int y)
    {
        return _map.IsWalkableByTileId(new Vector2(x, y));
    }

    void AddToOpenList(Tile tile, Tile parent, float g, float h)
    {
        float f = g + h;

        if(tile.IsOpen)
        {
            if(tile.F > f)
            {
                tile.Parent = parent;
                tile.G = g;
                tile.H = h;
            }
        }
        else
        {
            tile.Parent = parent;
            tile.G = g;
            tile.H = h;

            bool added = false;
            for (int i = 0; i < _openList.Count; ++i)
            {
                if(_openList[i].F > f)
                {
                    _openList.Insert(i, tile);

                    added = true;
                    break;
                }
            }

            if(!added)
            {
                _openList.Add(tile);
            }
        }

        tile.IsOpen = true;
    }

    void BuildPath(Tile tile)
    {
        _path.Clear();

        while(tile != null)
        {
            tile.IsInPath = true;
            _path.Add(tile.WorldPos);
            tile = tile.Parent;
        }
    }

    void ResetTileMap()
    {
        _map.ResetPathfindingValues();
    }
}
