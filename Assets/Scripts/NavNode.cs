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
    public List<NavNode> neighbours;
    public NavNode parent;
    public InteractableObject InteractableObject;
    public Mob Mob;

    int heapIndex;
    bool _visible = true;
    bool _hasBeenSeen = false;
    bool _walkable = true;
    bool _blocked = false;
    bool _blocksLight = false;

    SpriteRenderer _spriteRenderer;

    public bool Walkable
    {
        get { return _walkable; }
        set {
            _walkable = value;
            UpdateLayer();
        }
    }

    public bool Blocked
    {
        get { return _blocked; }
        set {
            _blocked = value;
        }
    }

    public bool BlocksLight
    {
        get { return _blocksLight; }
        set {
            _blocksLight = value;
            UpdateLayer();
        }
    }

    public bool HasBeenSeen
    {
        get { return _hasBeenSeen; }
    }

    public bool Visible
    {
        get { return _visible; }
        set {
            _visible = value;
            if (_visible)
            {
                _hasBeenSeen = true;
            }

            // set tile shadow color
            if (!_visible)
            {
                if (_hasBeenSeen)
                {
                    _spriteRenderer.color = new Color(0, 0, 0, 0.5f); // remembered
                }
                else
                {
                    _spriteRenderer.color = new Color(0, 0, 0, 1); // blacked out
                }
            }
            else
            {
                _spriteRenderer.color = new Color(0, 0, 0, 0); // visible
            }
        }
    }

    public Vector2 WorldPosition
    {
        get { return new Vector2(worldPosX, worldPosY); }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
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

    private void UpdateLayer()
    {
        if (!_walkable || _blocksLight)
        {
            gameObject.layer = LayerMask.NameToLayer("NavNode_Wall");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("NavNode_Floor");
        }
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
        Gizmos.color = _walkable ? Color.white : Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.9f, 0.9f, 0.9f));
    }
}
