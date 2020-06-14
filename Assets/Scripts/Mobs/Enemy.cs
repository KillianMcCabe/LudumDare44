using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Mob
{
    enum State
    {
        Sleeping,
        Wandering,
        Aggravated
    }

    private bool _isMoving;
    public bool IsMoving
    {
        get { return _isMoving; }
    }

    public int strength;
    public int armor;
    public int health;

    private State _state;
    private NavNode currentNodePosition;
    private List<NavNode> pathing;
    Coroutine pathingCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        currentNodePosition = NavigationGrid.Instance.GetNode(new Vector2(transform.position.x, transform.position.y));
        currentNodePosition.Mob = this;

        GameManager.Instance.AddEnemyToList(this);

        _state = State.Sleeping;
        _isMoving = false;
    }

    public void Move()
    {
        switch (_state)
        {
            case State.Sleeping:
                break;
            case State.Wandering:
                break;
            case State.Aggravated:

                _isMoving = true;

                pathing = Pathing.CalculatePath(currentNodePosition, Player.Instance.NodePosition);
                if (pathingCoroutine != null)
                {
                    StopCoroutine(pathingCoroutine);
                }
                pathingCoroutine = StartCoroutine(PathingCoroutine());
                
                break;
        }
    }

    public override void ReceiveAttack(int attackPower)
    {
        _state = State.Aggravated;

        int damage = Mathf.Max(attackPower - armor, 0);
        health -= damage;
        MessageLogController.Instance.AddMessage($"You dealt {damage} damage to the Slime.");
        if (health <= 0)
        {
            MessageLogController.Instance.AddMessage("You killed the Slime.");
            currentNodePosition.Mob = null;
            GameManager.Instance.RemoveEnemyFromList(this);
            GameObject.Destroy(gameObject);
        }
    }

    // Update is called once per frame
    private IEnumerator PathingCoroutine()
    {
        while (pathing.Count > 0)
        {
            // pop node
            NavNode nextNode = pathing[0];
            pathing.RemoveAt(0);

            // check if the path is blocked by an object e.g. door
            if (nextNode.InteractableObject != null && nextNode.Blocked)
            {
                _isMoving = false;
                yield break;
            }
            // check if the path is blocked by a mob
            else if (nextNode.Mob != null)
            {
                // is the mob a player?
                if (nextNode.Mob is Player)
                {
                    // attack the player
                    nextNode.Mob.ReceiveAttack(strength);
                }
                _isMoving = false;
                yield break;
            }
            else
            {
                currentNodePosition.Mob = null;
                currentNodePosition = nextNode;
                currentNodePosition.Mob = this;
                // transform.position = currentNodePosition.WorldPosition;
            }
            yield return new WaitForSeconds(0.25f);
        }

        _isMoving = false;
    }
}
