using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField]
    SpriteRenderer _spriteRenderer = null;

    public int armor;
    public int health;

    private NavNode currentNodePosition;

    // Start is called before the first frame update
    void Start()
    {
        currentNodePosition = NavigationGrid.Instance.GetNode(new Vector2(transform.position.x, transform.position.y));
        Debug.Log("locked door is at " + currentNodePosition.WorldPosition);
        currentNodePosition.Enemy = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DealDamage(int attackPower)
    {
        int damage = Mathf.Max(attackPower - armor, 0);
        health -= damage;
        MessageLogController.Instance.AddMessage($"You dealt {damage} damage to the Slime");
        if (health <= 0)
        {
            MessageLogController.Instance.AddMessage("You killed the Slime");
            currentNodePosition.Enemy = null;
            GameObject.Destroy(gameObject);
        }
    }
}
