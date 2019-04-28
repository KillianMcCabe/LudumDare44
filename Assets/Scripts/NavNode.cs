using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavNode : MonoBehaviour , IHeapItem<NavNode>
{
    public static Action<NavNode> OnNodeClicked;

    public int gCost;
    public int hCost;
    public int gridX, gridY;
    public int worldPosX, worldPosY;
    public bool walkable = true;
    public List<NavNode> neighbours;
    public NavNode parent;
    public InteractableObject InteractableObject;
    public Enemy Enemy;
    int heapIndex;

    public Vector2 WorldPosition
    {
        get { return new Vector2(worldPosX, worldPosY); }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Start()
    {
        if (walkable)
        {
            BoxCollider2D box = gameObject.AddComponent<BoxCollider2D>();
            box.size = new Vector2(1, 1);
        }
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(NavNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }

    void OnMouseDown()
    {
        if (OnNodeClicked != null)
        {
            OnNodeClicked.Invoke(this);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = walkable ? Color.white : Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.9f, 0.9f, 0.9f));
        // Gizmos.DrawSphere(transform.position, 0.25f);
    }
}
