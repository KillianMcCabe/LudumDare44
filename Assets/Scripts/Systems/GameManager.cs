using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public static Player LocalPlayer = null;

    public float levelStartDelay = 2f;                      // Time to wait before starting level, in seconds.
    public float turnDelay = 0.1f;                          // Delay between each Player turn.
    public int playerFoodPoints = 100;                      // Starting value for Player food points.

    [System.NonSerialized]
    public bool playersTurn = true;                         // Boolean to check if it's players turn, hidden in inspector but public.

    [SerializeField]
    private NavigationGrid _navigationGrid = null;

    [SerializeField]
    private GameObject _enemiesMovingPanel = null;

    [SerializeField]
    private Transform[] _playerSpawnPositions = null;

    private List<Enemy> enemies = new List<Enemy>();        // List of all Enemy units, used to issue them move commands.
    private bool enemiesMoving;                             // Boolean to check if enemies are moving.

    private void Awake()
    {
        InitGame();
    }

    private IEnumerator Start()
    {
        // Wait for local player to be assigned
        while (LocalPlayer == null)
            yield return null;

        NavigationGrid.Instance.Lighting.Recalculate();
        LocalPlayer.StartTurn();
    }

    private void Update()
    {
        //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
        if (playersTurn || enemiesMoving)
            return;

        StartCoroutine(MoveEnemies());
    }

    // Call this to add the passed in Enemy to the List of Enemy objects.
    public void AddEnemyToList(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemyFromList(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    // GameOver is called when the player reaches 0 food points
    public void GameOver()
    {
        //Disable this GameManager.
        enabled = false;
    }

    private void InitGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Do setup..

            // Create a networked player object for each player that loads into the multiplayer scenes.
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Photon.Realtime.Player player = PhotonNetwork.PlayerList[i];

                // Instantiate the Player prefab on every client
                GameObject go = PhotonNetwork.Instantiate(
                    Path.Combine("PhotonPrefabs", "Player"),
                    _playerSpawnPositions[i].position,
                    _playerSpawnPositions[i].rotation
                );

                PhotonView pv = go.GetComponent<PhotonView>();
                pv.TransferOwnership(player);
                Debug.Log("1 - transfered ownership");
            }
        }
        
        // Clear any Enemy objects in our List to prepare for next level.
        enemies.Clear();
    }

    // Coroutine to move enemies in sequence.
    private IEnumerator MoveEnemies()
    {
        // While enemiesMoving is true player is unable to move.
        enemiesMoving = true;

        // If there are no enemies spawned (IE in first level):
        if (enemies.Count == 0)
        {
            // Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
            yield return new WaitForSeconds(turnDelay);
        }
        else
        {
            _enemiesMovingPanel.SetActive(true);
        }

        // Loop through List of Enemy objects.
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].Move();

            // Wait for Enemy's moveTime before moving next Enemy,
            while (enemies[i].IsMoving)
            {
                yield return null;
            }
        }

        // Enemies are done moving, set playersTurn to true so player can move.
        playersTurn = true;
        LocalPlayer.StartTurn();

        // Enemies are done moving, set enemiesMoving to false.
        enemiesMoving = false;

        _enemiesMovingPanel.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if (_playerSpawnPositions != null)
        {
            Vector3 size = new Vector3 (0.5f, 0.5f, 0.5f);
            for (int i = 0; i < _playerSpawnPositions.Length; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(_playerSpawnPositions[i].position, size);
            }
        }
    }
}