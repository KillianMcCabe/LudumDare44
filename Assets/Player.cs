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

    // Start is called before the first frame update
    void Start()
    {
        // get current position on node grid
        currentNodePosition = NavigationGrid.Instance.GetNode(new Vector2(transform.position.x, transform.position.y));
        Debug.Log(currentNodePosition);
        NavNode.OnNodeClicked += HandleNodeClicked;
    }

    // Update is called once per frame
    IEnumerator Pathing()
    {
        while (pathing.Count > 0)
        {
            currentNodePosition = pathing[0];
            transform.position = currentNodePosition.WorldPosition;
            pathing.RemoveAt(0);
            yield return new WaitForSeconds(0.25f);
        }
    }

    void HandleNodeClicked(NavNode clickedNavNode)
    {
        pathing = NavigationGrid.Instance.CalculatePath(currentNodePosition, clickedNavNode);
        // foreach (NavNode n in pathing)
        // {
        //     Debug.Log(n.WorldPosition);
        // }

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
