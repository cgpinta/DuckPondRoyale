using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SSSBoard : MonoBehaviour
{

    public SSSIcon SSSIconPrefab;
    public List<StageSettings> stages;

    LobbyManager lobbyManager;

    // Start is called before the first frame update
    void Start()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
        lobbyManager.RoomLoaded += displaySSS;
    }

    public void displaySSS()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            stages = FindObjectOfType<LobbyManager>().stageList.getList;

            foreach (StageSettings stage in stages)
            {
                SSSIcon newIcon = Instantiate(SSSIconPrefab, this.transform);
                newIcon.settings = stage;
                newIcon.GetPropertiesFromSettings();
            }
        }
    }
}
