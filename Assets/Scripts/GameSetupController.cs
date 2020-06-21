using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// This script will be added to any multiplayer scene
public class GameSetupController : MonoBehaviour
{
    enum GamePhase
    {
        None,
        Setup,
        OutOfCombat,
        Dialogue,
        Combat
    }

    private GamePhase _gamePhase;


    void Awake()
    {
        // Create a networked player object for each player that loads into the multiplayer scenes.
        EnsureLocalPlayerExists();

        _gamePhase = GamePhase.Setup;
    }

    void Update()
    {
        switch (_gamePhase)
        {
            case GamePhase.Setup:
                GameSetup();
                break;
        }
    }

    private void EnsureLocalPlayerExists()
    {
        if (PlayerManager.LocalPlayerInstance == null)
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

            // Instantiate the PhotonPlayer prefab on every client
            GameObject gameObject = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer"), Vector3.zero, Quaternion.identity);
            PlayerManager player = gameObject.GetComponent<PlayerManager>();

            PlayerManager.LocalPlayerInstance = player;
        }
        else
        {
            Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
        }
    }

    public void TakeDamage()
    {
        PlayerManager.LocalPlayerInstance.Health --;
    }

    private void GameSetup()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PlayerManager.LocalPlayerInstance != null)
            {
                // _playerInfo.SetPlayer(PlayerManager.LocalPlayerInstance);
            }

            // do setup..

            _gamePhase = GamePhase.OutOfCombat;
        }
        else
        {
            // wait for master client to complete setup
        }
    }
}
