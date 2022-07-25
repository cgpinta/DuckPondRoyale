using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    

    public GameObject lobbyPanel;
    public TMP_InputField createInput;

    public GameObject roomPanel;
    public TMP_Text roomName;
    public RoomItem roomItemPrefab;
    List<RoomItem> roomItems = new List<RoomItem>();
    public Transform contentObject;

    Timer roomListUpdateCooldown = new Timer();
    float timeBetweenRLUpdates = 1.5f;

    public List<PlayerItem> playerItems = new List<PlayerItem>();
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;

    private void Start()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }


    public void OnClickCreate()
    {
        if(createInput.text.Length > 0)
        {
            PhotonNetwork.CreateRoom(createInput.text, new RoomOptions() { MaxPlayers = 10});
        }
    }

    public void JoinRoom()
    {
        //PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (!roomListUpdateCooldown.isInProgress())
        {
            UpdateRoomList(roomList);
            roomListUpdateCooldown.setTimer(timeBetweenRLUpdates);
        }
    }

    void UpdateRoomList(List<RoomInfo> list)
    {
        foreach(RoomItem roomItem in roomItems)         //remove all current rooms from list
        {
            Destroy(roomItem.gameObject);
        }
        roomItems.Clear();

        foreach(RoomInfo room in list)                  //add updated list to list of rooms
        {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItems.Add(newRoom);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    void UpdatePlayerList()
    {
        foreach (PlayerItem playerItem in playerItems)         //remove all current rooms from list
        {
            Destroy(playerItem.gameObject);
        }
        playerItems.Clear();

        if(PhotonNetwork.CurrentRoom == null) { return; }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);
            playerItems.Add(newPlayerItem);
        }

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }
}
