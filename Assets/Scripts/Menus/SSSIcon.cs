using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SSSIcon : MonoBehaviour
{
    public StageSettings settings;

    GameObject charIcon;
    public Image charIconImg;

    LobbyManager LobbyManager;

    private void Start()
    {
        LobbyManager = FindObjectOfType<LobbyManager>();
    }

    public void GetPropertiesFromSettings()
    {
        charIconImg.sprite = settings.SSS;
    }

    public void onClickSetStage()
    {
        LobbyManager.OnClickSetStage(settings);
        LobbyManager.OnClickSetStage(settings);    //method called twice bc doesnt always work when called once
    }
}
