using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RoomManager : MonoBehaviour
{

    public PlayerItem playerItemPrefab;
    public List<PlayerItem> playerItems = new List<PlayerItem>();
    public Transform playerTransform;




    void UpdatePlayerList()
    {
        foreach(PlayerItem item in playerItems)
        {
            Destroy(item.gameObject);
        }
        playerItems.Clear();

        if(PhotonNetwork.CurrentRoom == null) { return; }

        //foreach(KeyValuePair<int, Player> )
    }
}
