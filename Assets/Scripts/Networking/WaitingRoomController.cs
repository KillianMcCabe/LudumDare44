using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PaperDungeons
{
    public class WaitingRoomController : MonoBehaviourPunCallbacks
    {
        private const string MainMenuSceneName = "Main";
        private const string GameSceneName = "Game";

        /*This object must be attached to an object
        / in the waiting room scene of your project.*/

        // photon view for sending rpc that updates the timer
        private PhotonView _myPhotonView = null;

        [Header("Prefabs")]

        [SerializeField]
        private GameObject _playerListingPrefab = null;

        [Header("External-components")]

        [SerializeField]
        private Transform _playersListContainer = null;

        // text variables for holding the displays for the countdown timer and player count
        [SerializeField]
        private Text _playerCountDisplay = null;
        [SerializeField]
        private Text _regionText = null;

        // number of players in the room out of the total room size
        private int _playerCount = 0;
        private int _roomSize = 0;

        private void Start()
        {
            // initialize variables
            _myPhotonView = GetComponent<PhotonView>();

            _regionText.text = "Region: " + PhotonNetwork.CloudRegion;

            // set max players to 4
            PhotonNetwork.CurrentRoom.MaxPlayers = 4;

            PlayerCountUpdate();
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player player)
        {
            Debug.Log(player.NickName + " has joined the room");
            PlayerCountUpdate();
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player player)
        {
            Debug.Log(player.NickName + " has left the room");
            PlayerCountUpdate();
        }

        /// <summary>
        /// Called from UI button when host player wants to start the game
        /// </summary>
        public void StartGame()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(GameSceneName);
        }

        /// <summary>
        /// Called from UI button when a player wants to leave the lobby
        /// </summary>
        public void Leave()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(MainMenuSceneName);
        }

        /// <summary>
        /// Called whenever a player joins or leaves the room
        /// </summary>
        private void PlayerCountUpdate()
        {
            // updates player count when players join the room
            _playerCount = PhotonNetwork.PlayerList.Length;
            _roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
            _playerCountDisplay.text = _playerCount + " / " + _roomSize;

            ListPlayers();
        }

        private void ListPlayers()
        {
            // clear old player listing
            for (int i = _playersListContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(_playersListContainer.GetChild(0).gameObject);
            }

            // generate new player listing
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList) //loop through each player and create a player listing
            {
                GameObject tempListing = Instantiate(_playerListingPrefab, _playersListContainer);
                Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
                tempText.text = player.NickName;
            }
        }
    }
}
