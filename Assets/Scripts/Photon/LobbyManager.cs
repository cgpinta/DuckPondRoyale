using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    

    [Header("Lobby Panel")]
    public GameObject lobbyPanel;
    public Transform roomPanelContextTR;            //transform of context object inside the room list in the lobby panel
    public TMP_InputField createInput;
    Timer roomListUpdateCooldown = new Timer();
    float timeBetweenRLUpdates = 1.5f;

    [Header("Room Panel")]
    public GameObject roomPanel;
    public TMP_Text roomName;
    public RoomItem roomItemPrefab;
    List<RoomItem> roomItems = new List<RoomItem>();
    public int minPlayerCount;

    

    public List<PlayerItem> playerItems = new List<PlayerItem>();
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;

    public List<Color32> playerOrderColors = new List<Color32>();

    public CharacterList characterList;

    public GameObject startButton;

    private void Start()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= minPlayerCount)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }

    public List<DuckSettings> GetCharacters()
    {
        return characterList.getList;
    }

    public void OnClickCreate()
    {
        if(createInput.text.Length > 0)
        {
            PhotonNetwork.CreateRoom(createInput.text, new RoomOptions() { MaxPlayers = 10, BroadcastPropsChangeToAll = true});
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
            RoomItem newRoom = Instantiate(roomItemPrefab, roomPanelContextTR);
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



            if(player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.ApplyLocalChanges();
            }

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

    public event Action<Player, DuckSettings> SetPlayerChar;
    public void OnClickSetPlayerChar(DuckSettings settings)
    {
        if (SetPlayerChar != null)
        {
            SetPlayerChar(PhotonNetwork.LocalPlayer, settings);
        }
    }

    public void OnClickStartButton()
    {
        PhotonNetwork.LoadLevel("Fight");
    }
}
