using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class WinScreen : MonoBehaviour
{
    public TMP_Text winnerName;
    public Image winnerCharImage;

    public Player player;

    public CharacterList characterList;

    public PlayerManager pManager;
    public Canvas WinCanvas;

    // Start is called before the first frame update
    void Start()
    {
        WinCanvas.worldCamera = Camera.main;
        pManager.PlayerWon += DisplayWinDetails;
    }

    public void DisplayWinDetails(PlayerController player)
    {
        if (PhotonNetwork.IsConnected)
        {
            winnerName.text = player.player.NickName;
            winnerCharImage.sprite = characterList.getList[(int)player.player.CustomProperties["SelectedChar"]].CSS;
        }
        else
        {
            winnerName.text = "no name";
            winnerCharImage.sprite = characterList.getList[0].CSS;
        }
        //WinCanvas.gameObject.SetActive(true);
    }
}
