using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomController : MonoBehaviourPunCallbacks
{
    private const string WaitinRoomSceneName = "WaitingRoom";

    public override void OnEnable()
    {
        //register to photon callback functions
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        //unregister to photon callback functions
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom() //Callback function for when we successfully create or join a room.
    {
        // called when our player joins the room
        // load into waiting room scene
        SceneManager.LoadScene(WaitinRoomSceneName);
    }
}
