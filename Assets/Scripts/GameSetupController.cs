using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSetupController : MonoBehaviour
{
    public static Player LocalPlayer = null;

    [SerializeField]
    private Transform[] _playerSpawnPositions = null;

    private void Awake()
    {
        // Create a networked player object for each player that loads into the multiplayer scenes.
        EnsureLocalPlayerExists();

        if (PhotonNetwork.IsMasterClient)
        {
            // do setup..
            
        }
    }

    private void EnsureLocalPlayerExists()
    {
        if (LocalPlayer == null)
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

            // Instantiate the Player prefab on every client
            GameObject go = PhotonNetwork.Instantiate(
                Path.Combine("PhotonPrefabs", "Player"),
                _playerSpawnPositions[0].position,
                _playerSpawnPositions[0].rotation
            );

            LocalPlayer = go.GetComponent<Player>();
        }
        else
        {
            Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
        }
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
