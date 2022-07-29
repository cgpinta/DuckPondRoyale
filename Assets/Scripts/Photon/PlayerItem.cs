using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviourPunCallbacks
{
    public TMP_Text playerName;

    public Image backgroundImage;
    public Color highlightColor;

    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
    public Image selectedCharacterIcon;

    Player player;

    LobbyManager LobbyManager;

    private void Awake()
    {
        LobbyManager = FindObjectOfType<LobbyManager>();
        LobbyManager.SetPlayerChar += SetPlayerSelectedChar;
    }

    private void Start()
    {
        backgroundImage = GetComponent<Image>();
        
    }

    public void SetPlayerInfo(Player player)
    {
        playerName.text = player.NickName;
        this.player = player;
        UpdatePlayerItem(player);
    }

    public void ApplyLocalChanges()
    {
        //backgroundImage.color = highlightColor;
    }

    public void SetPlayerSelectedChar(Player selPlayer, DuckSettings duck)
    {
        Debug.Log(duck.name+" was selected");
        if(selPlayer != player) { return; }
        playerProperties["SelectedChar"] = duck.ID;
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
        //selectedCharacterIcon.sprite = duck.CSS;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(targetPlayer == player)
        {
            UpdatePlayerItem(player);
            
        }
    }

    void UpdatePlayerItem(Player player)
    {
        if (player.CustomProperties.ContainsKey("SelectedChar"))
        {
            DuckSettings temp = LobbyManager.GetCharacters()[(int)playerProperties["SelectedChar"]];
            selectedCharacterIcon.sprite = temp.CSS;
            playerProperties["SelectedChar"] = player.CustomProperties["SelectedChar"];
        }
        else
        {
            playerProperties["SelectedChar"] = -1;
        }
    }
}
