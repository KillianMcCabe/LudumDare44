using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightNode
{
    public List<NavNode> navNodes;
    public Vector2 WorldPosition;

    bool _visible;

    public bool Visible
    {
        get { return _visible; }
        set {
            _visible = value;
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(new Vector3(WorldPosition.x, WorldPosition.y, 0), 0.5f);
    }
}
