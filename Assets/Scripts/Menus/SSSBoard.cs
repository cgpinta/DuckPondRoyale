using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SSSBoard : MonoBehaviourPunCallbacks
{

    public SSSIcon SSSIconPrefab;
    public List<StageSettings> stages;

    LobbyManager lobbyManager;

    // Start is called before the first frame update
    void Start()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
        //lobbyManager.RoomLoaded += displaySSS;

        stages = FindObjectOfType<LobbyManager>().stageList.getList;

        foreach (StageSettings stage in stages)
        {
            SSSIcon newIcon = Instantiate(SSSIconPrefab, this.transform);
            newIcon.settings = stage;
            newIcon.GetPropertiesFromSettings();
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
