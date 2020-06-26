using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace PaperDungeons
{
    public class LobbyController : MonoBehaviourPunCallbacks
    {
        private const int RoomSize = 2; // The max number of player in the room at one time.

        [SerializeField]
        private InputField _playerNickNameInput = null; //Input field so player can change their NickName

        [SerializeField]
        private Button _joinRoomButton = null; // button used for creating and joining a game.

        [SerializeField]
        private Button _cancelJoinRoomButton = null; // button used to stop searing for a game to join.

        private void Start()
        {
            _playerNickNameInput.onValueChanged.AddListener(HandlePlayerNameUpdateInputChanged);
        }

        public override void OnConnectedToMaster() // Callback function for when the first connection is established successfully.
        {
            PhotonNetwork.AutomaticallySyncScene = true; // Makes it so whatever scene the master client has loaded is the scene all other clients will load
            _joinRoomButton.gameObject.SetActive(true);

            // check for player name saved to player prefs
            if (PlayerPrefs.HasKey("NickName"))
            {
                if (PlayerPrefs.GetString("NickName") == "")
                {
                    PhotonNetwork.NickName = "Player " + Random.Range(0, 1000); //random player name when not set
                }
                else
                {
                    PhotonNetwork.NickName = PlayerPrefs.GetString("NickName"); //get saved player name
                }
            }
            else
            {
                PhotonNetwork.NickName = "Player " + Random.Range(0, 1000); //random player name when not set
            }
            _playerNickNameInput.text = PhotonNetwork.NickName; //update input field with player name
        }

        public void JoinRoom() // Paired to the JoinRoon button
        {
            _joinRoomButton.gameObject.SetActive(false);
            _cancelJoinRoomButton.gameObject.SetActive(true);

            Debug.Log("Searching for room to join");
            PhotonNetwork.JoinRandomRoom(); //First tries to join an existing room
        }

        public void CancelJoinRoom() //Paired to the cancel button. Used to stop looking for a room to join.
        {
            _cancelJoinRoomButton.gameObject.SetActive(false);
            _joinRoomButton.gameObject.SetActive(true);
            PhotonNetwork.LeaveRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            CreateRoom(); // if it fails to join a room then it will try to create its own
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Failed to create room... trying again");
            CreateRoom(); //Retrying to create a new room with a different name.
        }

        private void CreateRoom()
        {
            Debug.Log("Creating room..");
            int randomRoomNumber = Random.Range(0, 10000); //creating a random name for the room
            RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)RoomSize };
            string roomName = "Room" + randomRoomNumber;
            PhotonNetwork.CreateRoom(roomName, roomOps); //attempting to create a new room
            Debug.Log($"Created room \"{roomName}\"");
        }

        private void HandlePlayerNameUpdateInputChanged(string nameInput) //input function for player name. paired to player name input field
        {
            PhotonNetwork.NickName = nameInput;
            PlayerPrefs.SetString("NickName", nameInput);
        }
    }
}
