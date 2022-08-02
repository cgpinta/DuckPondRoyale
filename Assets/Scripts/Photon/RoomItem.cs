using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomItem : MonoBehaviour
{
    public TMP_Text roomName;
    public TMP_Text playerCount;
    LobbyManager manager;

    private void Start()
    {
        manager = FindObjectOfType<LobbyManager>();
    }
    public void SetRoomName(string newRoomName)
    {
        roomName.text = newRoomName;
    }
    public void SetPlayerCount(int newPlayerCount, int maxPlayerCount)
    {
        playerCount.text = newPlayerCount.ToString()+"/"+ maxPlayerCount;
    }

    public void OnClickItem()
    {
        manager.JoinRoom(roomName.text);
    }
}
