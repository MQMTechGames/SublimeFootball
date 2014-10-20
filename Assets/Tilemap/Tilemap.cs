using UnityEngine;
using System.Collections.Generic;

public class Tilemap : MonoBehaviour
{
    [SerializeField]
    GameObject _tilePrefab;

    [SerializeField]
    public Vector2 _nTiles = new Vector2(250, 250);

    [SerializeField]
    float _tileSize = 1;

    private List<Tile> _tiles = new List<Tile>();
    public List<Tile> Tiles{ get { return _tiles; } }

    public bool ShowGizmos = true;

    private Vector3 StartTilePosition
    {
        get
        {
            return transform.position - new Vector3(_nTiles.x, 0, _nTiles.y) * 0.5f * _tileSize;
        }
    }

    private Vector3 MinPosition
    {
        get
        {
            return StartTilePosition - new Vector3(1f, 0f, 1f) * _tileSize * 0.5f;
        }
    }

    void Start()
    {
        for (int y = 0; y < _nTiles.y; ++y)
        {
            for (int x = 0; x < _nTiles.x; ++x)
            {
                Vector2 tileId = new Vector2(x, y);
                Vector3 currPosition = StartTilePosition + new Vector3(x, 0f, y) * _tileSize;

                //GameObject go = new GameObject("Tile_" + GetIdxFromTileId(tileId));
                GameObject go = (GameObject) Instantiate(_tilePrefab, currPosition, Quaternion.identity);
                go.transform.localScale = new Vector3(0.9f, 0.9f ,0.9f);
                Tile tile = go.GetComponent<Tile>();

                _tiles.Add(tile);

                tile.Init(tileId, currPosition);
            }
        }
    }

    public int GetIdxFromTileId(Vector2 tileId)
    {
        return ((int)_nTiles.x) * (int)tileId.y + (int)tileId.x;
    }

    public Tile GetTileByTileIdx(int idx)
    {
        if(idx < 0 || idx >= _tiles.Count)
        {
            return null;
        }

        return _tiles[idx];
    }

    public Tile GetTileByTileId(Vector2 tileId)
    {
        if(   tileId.x < 0 || tileId.x >= _nTiles.x
           || tileId.y < 0 || tileId.y >= _nTiles.y
           )
        {
            return null;
        }
        
        int idx = GetIdxFromTileId(tileId);
        
        return GetTileByTileIdx(idx);
    }

    public Tile GetTileByWorldPos(Vector3 worldPos)
    {
        Vector2 tileId = GetTileIdByWorldPos(worldPos);

        return GetTileByTileId(tileId);
    }

    public Vector2 GetTileIdByWorldPos(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - MinPosition;
        
        Vector2 tileId = new Vector2(Mathf.FloorToInt(localPos.x / _tileSize), Mathf.FloorToInt(localPos.z / _tileSize));
        
        return tileId;
    }

    public bool IsWalkableByTileId(Vector2 tileId)
    {
        Tile tile = GetTileByTileId(tileId);

        if(tile == null)
        {
            return false;
        }

        return tile.IsWalkable;
    }

    public void GetNeighborsIds(Vector2 tileId, List<Vector2> neighbors)
    {
        neighbors.Add(tileId + new Vector2(1, 0));
        neighbors.Add(tileId + new Vector2(1, 1));
        neighbors.Add(tileId + new Vector2(1, -1));

        neighbors.Add(tileId + new Vector2(-1, 0));
        neighbors.Add(tileId + new Vector2(-1, 1));
        neighbors.Add(tileId + new Vector2(-1, -1));

        neighbors.Add(tileId + new Vector2(0, 1));
        neighbors.Add(tileId + new Vector2(0, -1));
    }

    public List<Vector2> GetNeighbors(Vector2 tileId)
    {
        List<Vector2> tilesIds = new List<Vector2>();

        int maxNumTiles = (int)_nTiles.x * (int)_nTiles.y;

        for (int y = -1; y < 2; ++y)
        {
            for (int x = -1; x < 2; ++x)
            {
                Vector2 currTileId = tileId + new Vector2(x, y);

                if(currTileId == tileId)
                {
                    continue;
                }

                if(   currTileId.x < 0 || currTileId.x >= _nTiles.x
                   || currTileId.y < 0 || currTileId.y >= _nTiles.y
                   )
                {
                    continue;
                }

                tilesIds.Add(currTileId);
            }
        }

        return tilesIds;
    }

    public void ResetPathfindingValues()
    {
        for (int i = 0; i < _tiles.Count; ++i)
        {
            _tiles[i].ResetPathfindingValues();
        }
    }

    void OnDrawGizmosSelected()
    {
        if(!ShowGizmos)
        {
            return;
        }

        for (int i = 0; i < _tiles.Count; ++i)
        {
            _tiles[i].DoOnDrawGizmos();
        }
    }
}
