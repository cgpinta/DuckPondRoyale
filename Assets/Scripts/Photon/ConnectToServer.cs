using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public TMP_InputField nicknameField;
    public TMP_Text connectButtonText;

    public void StartConnection()
    {
        if(nicknameField.text.Length < 1)
        {
            return;
        }
        
        PhotonNetwork.NickName = nicknameField.text;
        connectButtonText.text = "Connecting...";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
