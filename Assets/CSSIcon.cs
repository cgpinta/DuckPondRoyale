using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CSSIcon : MonoBehaviour
{
    public DuckSettings settings;

    GameObject charIcon;
    public Image charIconImg;

    LobbyManager LobbyManager;

    private void Start()
    {
        LobbyManager = FindObjectOfType<LobbyManager>();
    }

    public void GetPropertiesFromSettings()
    {
        charIconImg.sprite = settings.CSS;
    }

    public void onClickSetPlayerChar()
    {
        LobbyManager.OnClickSetPlayerChar(settings);
        LobbyManager.OnClickSetPlayerChar(settings);
    }
}
