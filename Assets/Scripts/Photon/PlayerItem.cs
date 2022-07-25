using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{
    public TMP_Text playerName;

    Image backgroundImage;
    public Color highlightColor;
    public Image currentCharacterSmall;

    public void SetPlayerInfo(Player player)
    {
        playerName.text = player.NickName;
    }

}
