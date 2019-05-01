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

    [SerializeField]
    SpriteRenderer _spriteRenderer = null;

    private bool isMoving;

    public bool IsMoving
    {
        get { return isMoving; }
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
        isMoving = false;
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

                isMoving = true;

                pathing = NavigationGrid.Instance.CalculatePath(currentNodePosition, Player.Instance.NodePosition);
                if (pathingCoroutine != null)
                {
                    StopCoroutine(pathingCoroutine);
                }
                pathingCoroutine = StartCoroutine(Pathing());
                
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
    private IEnumerator Pathing()
    {
        while (pathing.Count > 0)
        {
            // pop node
            NavNode nextNode = pathing[0];
            pathing.RemoveAt(0);

            // check if the path is blocked by an object e.g. door
            if (nextNode.InteractableObject != null && nextNode.Blocked)
            {
                isMoving = false;
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
                isMoving = false;
                yield break;
            }
            else
            {
                currentNodePosition.Mob = null;
                currentNodePosition = nextNode;
                currentNodePosition.Mob = this;
                transform.position = currentNodePosition.WorldPosition;
            }
            yield return new WaitForSeconds(0.25f);
        }

        isMoving = false;
    }
}
