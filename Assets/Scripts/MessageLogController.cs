using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageLogController : MonoBehaviour
{
    [SerializeField]
    GameObject _messageList;

    [SerializeField]
    GameObject _messageLogItemPrefab;

    public static MessageLogController Instance;

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

    public void AddMessage(string message)
    {
        Debug.Log(message);
        GameObject newMessage = GameObject.Instantiate(_messageLogItemPrefab);
        newMessage.transform.SetParent(_messageList.transform);
        newMessage.transform.SetAsLastSibling();
        // newMessage.GetComponent<Text>().text = message;
        newMessage.GetComponent<TMPro.TMP_Text>().text = message;
    }
}
