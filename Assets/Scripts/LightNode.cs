using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LightNode
{
    public List<NavNode> navNodes;
    public Vector2 WorldPosition
    {
        get {return _worldPosition; }
    }

    bool _visible;
    // Tilemap _fogTilemap;
    // TileBase _fogTile;
    Vector2 _worldPosition;

    public bool Visible
    {
        get { return _visible; }
        set {
            _visible = value;
            if (_visible)
            {
                // Debug.Log("removing fog at " + WorldPositionVector3);
                // remove fog tile
                // _fogTilemap.SetTile(WorldPositionVector3Int, null);
            }
        }
    }

    public Vector3Int WorldPositionVector3Int
    {
        get { return new Vector3Int((int)_worldPosition.x, (int)_worldPosition.y, 0); }
    }

    public void Init(Tilemap fogTilemap, TileBase fogTile, Vector2 worldPos)
    {
        // _fogTilemap = fogTilemap;
        // _fogTile = fogTile;
        _worldPosition = worldPos;

        // _fogTilemap.SetTile(WorldPositionVector3Int, _fogTile);

        Visible = false;
    }
}
