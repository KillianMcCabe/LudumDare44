using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

namespace PaperDungeons
{
    public class Player : Mob
    {
        private enum State
        {
            None,
            WaitingForTurn,
            DecidingWhereToMoveOrAct,
            Moving,
            Acting
        }

        private NavNode _clickedNavNode;
        private List<NavNode> pathing;
        private List<NavNode> _nodesWithinMovementRange;
        private List<NavNode> _nodesWithinView;
        private Coroutine pathingCoroutine;

        public bool HasKey = false;
        public bool acceptingInput = true;

        // stats
        private int speed = 6;
        private int strength = 5;
        private int armor = 2;
        private int health = 10;

        private State _state;
        private int movementRemaining = 0;
        private bool inCombat = false;

        protected override void Awake()
        {
            base.Awake();

            if (_photonView.IsMine)
            {
                acceptingInput = true;

                NavNode.OnNodeClicked += HandleNodeClicked;
            }
        }

        protected override void Start()
        {
            base.Start();

            Debug.Log("Player \"" + _photonView.Owner.NickName + "\" is at " + NodePosition.name);
        }

        private void SetState(State state)
        {
            _state = state;

            switch (_state)
            {
                case State.DecidingWhereToMoveOrAct:
                    movementRemaining = speed;
                    _nodesWithinMovementRange = Pathing.GetNodesWithinRange(NodePosition, movementRemaining);
                    // show highlight on selectable nodes within movement range
                    for (int i = 0; i < _nodesWithinMovementRange.Count; i++)
                    {
                        _nodesWithinMovementRange[i].Highlight = Color.yellow;
                    }

                    _nodesWithinView = Pathing.GetNodesWithinRange(NodePosition, movementRemaining);
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

        private IEnumerator PathingCoroutine()
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
                else if (nextNode.Mob != null && nextNode.Mob is Enemy)
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
                        transform.position = Vector2.MoveTowards(transform.position, nextNode.WorldPosition, Time.deltaTime * MoveSpeed);
                        yield return null;
                    }

                    NodePosition = nextNode;
                }

                NavigationGrid.Instance.Lighting.Recalculate(this); // TODO: move into GameManager.cs
            }

            // check if we stopped on an item
            if (NodePosition.InteractableObject != null)
            {
                NodePosition.InteractableObject.Interact();
            }
            // check if we clicked on an item which is now in interaction range
            else if (_clickedNavNode.InteractableObject != null && NodePosition.neighbours.Contains(_clickedNavNode))
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

            // when in combat, only consider nodes within movement range
            if (inCombat)
            {
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
            }
            else
            {
                moveToThisNode = clickedNavNode;
            }

            if (moveToThisNode == null)
            {
                Debug.LogError("moveToThisNode is null!");
                return;
            }

            // Debug.Log("Moving to " + moveToThisNode.name);

            pathing = Pathing.CalculatePath(NodePosition, moveToThisNode);
            if (pathing != null)
            {
                if (pathingCoroutine != null)
                {
                    StopCoroutine(pathingCoroutine);
                }

                SetState(State.Moving);
                pathingCoroutine = StartCoroutine(PathingCoroutine());
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
                        Gizmos.DrawLine(NodePosition.WorldPositionVector3, pathing[i].WorldPositionVector3);
                    }
                    else
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine(pathing[i-1].WorldPositionVector3, pathing[i].WorldPositionVector3);
                    }
                }
            }
        }
    }
}