using UnityEngine;
using System.Collections;

using System.Collections.Generic;       //Allows us to use Lists. 

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;                      //Time to wait before starting level, in seconds.
    public float turnDelay = 0.1f;                          //Delay between each Player turn.
    public int playerFoodPoints = 100;                      //Starting value for Player food points.
    public static GameManager Instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    [HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.

    [SerializeField]
    GameObject _enemiesMovingPanel;

    private List<Enemy> enemies;                          //List of all Enemy units, used to issue them move commands.
    private bool enemiesMoving;                             //Boolean to check if enemies are moving.

    //Awake is always called before any Start functions
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

        //Assign enemies to a new List of Enemy objects.
        enemies = new List<Enemy>();

        //Call the InitGame function to initialize the first level 
        InitGame();
    }

    //Initializes the game for each level.
    void InitGame()
    {
        //Clear any Enemy objects in our List to prepare for next level.
        enemies.Clear();

        // _navigationGrid.GenerateNodes(level);
    }

    IEnumerator Start()
    {
        yield return null;
        NavigationGrid.Instance.CalculateLighting();
    }

    //Update is called every frame.
    void Update()
    {
        //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
        if (playersTurn || enemiesMoving)
            return;

        StartCoroutine(MoveEnemies());
    }

    //Call this to add the passed in Enemy to the List of Enemy objects.
    public void AddEnemyToList(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemyFromList(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    //GameOver is called when the player reaches 0 food points
    public void GameOver()
    {
        //Enable black background image gameObject.
        // levelImage.SetActive(true);

        //Disable this GameManager.
        enabled = false;
    }

    //Coroutine to move enemies in sequence.
    IEnumerator MoveEnemies()
    {
        //While enemiesMoving is true player is unable to move.
        enemiesMoving = true;

        //If there are no enemies spawned (IE in first level):
        if (enemies.Count == 0)
        {
            //Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
            yield return new WaitForSeconds(turnDelay);
        }
        else
        {
            _enemiesMovingPanel.SetActive(true);
        }

        //Loop through List of Enemy objects.
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].Move();

            //Wait for Enemy's moveTime before moving next Enemy,
            while (enemies[i].IsMoving)
            {
                yield return null;
            }
        }

        // Enemies are done moving, set playersTurn to true so player can move.
        playersTurn = true;
        Player.Instance.StartTurn();

        // Enemies are done moving, set enemiesMoving to false.
        enemiesMoving = false;

        _enemiesMovingPanel.SetActive(false);
    }
}