using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : Mob
{
    enum State
    {
        None,
        WaitingForTurn,
        DecidingWhereToMoveOrAct,
        Moving,
        Acting
    }

    const float moveSpeed = 6f;

    NavNode currentNodePosition;
    NavNode _clickedNavNode;
    List<NavNode> pathing;
    List<NavNode> _nodesWithinMovementRange;
    Coroutine pathingCoroutine;

    public bool HasKey = false;
    public static Player Instance;
    public bool acceptingInput = true;

    int speed = 6;
    int strength = 5;
    int armor = 2;
    int health = 10;

    private State _state;
    private int movementRemaining = 0;

    public NavNode NodePosition
    {
        get { return currentNodePosition; }
    }

    public Vector2 WorldPosition
    {
        get { return transform.position; }
        // get { return currentNodePosition.WorldPosition; }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // get current position on node grid
        currentNodePosition = NavigationGrid.Instance.GetNode(new Vector2(transform.position.x, transform.position.y));
        currentNodePosition.Mob = this;

        Debug.Log("Player is at " + currentNodePosition);

        acceptingInput = true;

        NavNode.OnNodeClicked += HandleNodeClicked;
    }

    private void SetState(State state)
    {
        _state = state;

        switch (_state)
        {
            case State.DecidingWhereToMoveOrAct:
                movementRemaining = speed;
                _nodesWithinMovementRange = NavigationGrid.Instance.GetNodesWithinRange(currentNodePosition, movementRemaining);
                // show highlight on selectable nodes within movement range
                for (int i = 0; i < _nodesWithinMovementRange.Count; i++)
                {
                    _nodesWithinMovementRange[i].Highlight = Color.yellow;
                }
                break;
        }
    }

    // TODO: GameManager should call this and all references to GameManager.Instance.playersTurn should be removed
    public void StartTurn()
    {
        // acceptingInput = true;
        SetState(State.DecidingWhereToMoveOrAct);
    }

    public override void ReceiveAttack(int attackPower)
    {
        int damage = Mathf.Max(attackPower - armor, 0);
        health -= damage;
        MessageLogController.Instance.AddMessage($"You received {damage} damage!", MessageLogController.MessageType.Warning);

        if (health <= 0)
        {
            MessageLogController.Instance.AddMessage("You died.", MessageLogController.MessageType.Warning);
            GameManager.Instance.GameOver();
        }
    }

    // Update is called once per frame
    private IEnumerator Pathing()
    {
        while (pathing.Count > 0)
        {
            // pop
            NavNode nextNode = pathing[0];
            pathing.RemoveAt(0);

            // check if the path is blocked by an object (e.g. a door)
            if (nextNode.InteractableObject != null && nextNode.Blocked)
            {
                Debug.Log("path is blocked");
                nextNode.InteractableObject.Interact();

                // notify GameManager that players turn has ended
                GameManager.Instance.playersTurn = false;

                yield break;
            }
            // check if the path is blocked by an enemy
            else if (nextNode.Mob != null)
            {
                nextNode.Mob.ReceiveAttack(strength);

                // notify GameManager that players turn has ended
                GameManager.Instance.playersTurn = false;

                yield break;
            }
            else
            {
                Vector3 moveToPosition = new Vector3(nextNode.WorldPosition.x, nextNode.WorldPosition.y, transform.position.z);
                // move to new position
                while (transform.position != moveToPosition)
                {
                    transform.position = Vector2.MoveTowards(transform.position, nextNode.WorldPosition, Time.deltaTime * moveSpeed);
                    yield return null;
                }

                currentNodePosition.Mob = null;
                currentNodePosition = nextNode;
                currentNodePosition.Mob = this;
            }

            NavigationGrid.Instance.Lighting.Recalculate(); // TODO: move into GameManager.cs
        }

        // check if we stopped on an item
        if (currentNodePosition.InteractableObject != null)
        {
            currentNodePosition.InteractableObject.Interact();
        }
        // check if we clicked on an item which is now in interaction range
        else if (_clickedNavNode.InteractableObject != null && currentNodePosition.neighbours.Contains(_clickedNavNode))
        {
            _clickedNavNode.InteractableObject.Interact();
        }

        SetState(State.DecidingWhereToMoveOrAct);
    }

    private void HandleNodeClicked(NavNode clickedNavNode)
    {
        // ignore any node clicks if we're not currently expecting map input
        if (_state != State.DecidingWhereToMoveOrAct || !acceptingInput)
            return;

        _clickedNavNode = clickedNavNode;

        if (_nodesWithinMovementRange == null || _nodesWithinMovementRange.Count <= 0)
        {
            Debug.LogError("_nodesWithinMovementRange is null / empty");
            return;
        }

        NavNode moveToThisNode = null;
        if (_nodesWithinMovementRange.Contains(clickedNavNode))
        {
            // move to node if it can be moved to
            moveToThisNode = clickedNavNode;
        }
        else
        {
            // find node closest node which can be move to
            float distanceToNearestNode = float.MaxValue;
            foreach (NavNode n in _nodesWithinMovementRange)
            {
                float dist = Vector2.Distance(n.WorldPosition, clickedNavNode.WorldPosition);
                if (dist < distanceToNearestNode)
                {
                    moveToThisNode = n;
                    distanceToNearestNode = dist;
                }
            }
        }

        if (moveToThisNode == null)
        {
            Debug.LogError("moveToThisNode is null!");
            return;
        }

        // Debug.Log("Moving to " + moveToThisNode.name);

        pathing = NavigationGrid.Instance.CalculatePath(currentNodePosition, moveToThisNode);
        if (pathing != null)
        {
            if (pathingCoroutine != null)
            {
                StopCoroutine(pathingCoroutine);
            }

            SetState(State.Moving);
            pathingCoroutine = StartCoroutine(Pathing());
        }
    }

    void OnDrawGizmos()
    {
        if (pathing != null)
        {
            for (int i = 0; i < pathing.Count; i++)
            {
                if (i == 0)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(currentNodePosition.WorldPositionVector3, pathing[i].WorldPositionVector3);
                }
                else
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(pathing[i-1].WorldPositionVector3, pathing[i].WorldPositionVector3);
                }
            }
        }

        // if (_nodesWithinMovementRange != null)
        // {
        //     for (int i = 0; i < _nodesWithinMovementRange.Count; i++)
        //     {
        //         Gizmos.color = Color.yellow;
        //         Gizmos.DrawCube(_nodesWithinMovementRange[i].WorldPosition, Vector3.one);
        //     }
        // }
    }
}
