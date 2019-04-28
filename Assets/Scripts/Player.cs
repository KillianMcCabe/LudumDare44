using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [SerializeField]
    GameObject _target;

    NavNode currentNodePosition;

    List<NavNode> pathing;
    Coroutine pathingCoroutine;

    public bool HasKey = false;
    public static Player Instance;

    int strength = 5;

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
        Debug.Log("Player is at " + currentNodePosition);
        NavNode.OnNodeClicked += HandleNodeClicked;
    }

    // Update is called once per frame
    IEnumerator Pathing()
    {
        while (pathing.Count > 0)
        {
            // pop
            NavNode nextNode = pathing[0];
            pathing.RemoveAt(0);

            // check if the path is blocked by an object e.g. door
            if (nextNode.InteractableObject != null && nextNode.InteractableObject.IsBlocking)
            {
                nextNode.InteractableObject.Interact();
                yield break;
            }
            // check if the path is blocked by an enemy
            else if (nextNode.Enemy != null)
            {
                nextNode.Enemy.DealDamage(strength);
                yield break;
            }
            else
            {
                currentNodePosition = nextNode;
                transform.position = currentNodePosition.WorldPosition;
            }
            yield return new WaitForSeconds(0.25f);
        }

        // check if we stopped on an item
        if (currentNodePosition.InteractableObject != null)
        {
            currentNodePosition.InteractableObject.Interact();
        }
    }

    void HandleNodeClicked(NavNode clickedNavNode)
    {
        pathing = NavigationGrid.Instance.CalculatePath(currentNodePosition, clickedNavNode);

        if (pathingCoroutine != null)
        {
            StopCoroutine(pathingCoroutine);
        }
        pathingCoroutine = StartCoroutine(Pathing());
    }

    void HandleDoorClicked(NavNode clickedNavNode)
    {
        pathing = NavigationGrid.Instance.CalculatePath(currentNodePosition, clickedNavNode);

        if (pathingCoroutine != null)
        {
            StopCoroutine(pathingCoroutine);
        }
        pathingCoroutine = StartCoroutine(Pathing());
    }

    void OnDrawGizmos()
    {
        if (pathing == null)
            return;

        for (int i = 0; i < pathing.Count; i++)
        {
            if (i == 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(currentNodePosition.WorldPosition, pathing[i].WorldPosition);
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(pathing[i-1].WorldPosition, pathing[i].WorldPosition);
            }
        }
    }
}
