﻿using UnityEngine;

namespace PaperDungeons
{
    public class MessageLogController : SingletonMonoBehaviour<MessageLogController>
    {
        [SerializeField]
        GameObject _messageList = null;

        [SerializeField]
        GameObject _messageLogItemPrefab = null;

        public enum MessageType
        {
            Info,
            Warning,
            Success
        }

        public void AddMessage(string message, MessageType messageType = MessageType.Info)
        {
            Debug.Log(message);

            GameObject newMessage = GameObject.Instantiate(_messageLogItemPrefab);
            newMessage.transform.SetParent(_messageList.transform);
            newMessage.transform.SetAsLastSibling();
            newMessage.GetComponent<TMPro.TMP_Text>().text = message;

            if (messageType == MessageType.Warning)
            {
                newMessage.GetComponent<TMPro.TMP_Text>().color = Color.red;
            }
            else if (messageType == MessageType.Success)
            {
                newMessage.GetComponent<TMPro.TMP_Text>().color = Color.green;
            }
        }
    }
}
