using System.Collections.Generic;
using UnityEngine;

public class LightNode
{
    public List<NavNode> navNodes;
    public Vector2 WorldPosition
    {
        get {return _worldPosition; }
    }

    bool _visible;
    Vector2 _worldPosition;

    public bool Visible
    {
        get { return _visible; }
        set { _visible = value; }
    }

    public Vector3Int WorldPositionVector3Int
    {
        get { return new Vector3Int((int)_worldPosition.x, (int)_worldPosition.y, 0); }
    }

    public void Init(Vector2 worldPos)
    {
        _worldPosition = worldPos;

        Visible = false;
    }
}
