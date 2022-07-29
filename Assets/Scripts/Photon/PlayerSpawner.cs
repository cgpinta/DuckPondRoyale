using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public CharacterList chars;
    Transform[] spawnPoints;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private void Start()
    {
        spawnPoints = GetComponentsInChildren<Transform>();

        int spawnPointRand = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnPointRand];
        string newPlayer = chars.getList[(int)PhotonNetwork.LocalPlayer.CustomProperties["SelectedChar"]].name;
        PhotonNetwork.Instantiate("DuckPrefabs/" + newPlayer, spawnPoint.position, Quaternion.identity);

        //Vector2 randomPosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        //GameObject newPlayer = PhotonNetwork.Instantiate("DuckPrefabs/"+playerPrefab.name, randomPosition, Quaternion.identity);
        //newPlayer.name = playerPrefab.name+": " +PhotonNetwork.LocalPlayer.NickName;

    }


}
