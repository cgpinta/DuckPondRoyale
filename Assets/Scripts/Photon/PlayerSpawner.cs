using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerSpawner : MonoBehaviour
{
    public CharacterList chars;

    [Header("Stage UI")]
    public TMP_Text countdownText;



    Transform[] spawnPoints;
    Player localPlayer;              //the local player
    ExitGames.Client.Photon.Hashtable currentPlayerProperties = new ExitGames.Client.Photon.Hashtable();

    bool allPlayersLoaded;      //true if all players are loaded into the scene
    bool playersSpawning;       //true if players are loaded, starts setting player spawnpoints
    bool playerSpawned;         //true if all players have spawned into the game
    private void Start()
    {
        countdownText.text = "";
        playersSpawning = false;
        playerSpawned = false;
        if (PhotonNetwork.IsConnected)
        {
            localPlayer = PhotonNetwork.LocalPlayer;
            spawnPoints = GetComponentsInChildren<Transform>();

            currentPlayerProperties["HasLoadedStage"] = true;
            localPlayer.SetCustomProperties(currentPlayerProperties);

        }
    }

    private void Update()
    {
        if (!playerSpawned)
        {
            SpawnPlayers();
            return;
        }
        
    }

    private void SpawnPlayers()
    {
        if (localPlayer.CustomProperties.ContainsKey("shouldSpawn") && !playerSpawned)
        {
            spawnSelf();
            playerSpawned = true;
        }
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (!allPlayersLoaded)
        {
            allPlayersLoaded = true;
            foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                Player currentPlayer = player.Value;
                if (!currentPlayer.CustomProperties.ContainsKey("HasLoadedStage"))
                {
                    allPlayersLoaded = false;
                }
            }
        }
        else
        {
            if (!playersSpawning)
            {
                playersSpawning = true;
                SetPlayerSpawnpoints();
            }
        }
    }


    private void SetPlayerSpawnpoints()
    {
        bool[] isSpawnPointTaken = new bool[spawnPoints.Length];
        int numSpawnPointsTaken = 0;

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            Player currentPlayer = player.Value;
            currentPlayerProperties = currentPlayer.CustomProperties;

            if (!currentPlayer.CustomProperties.ContainsKey("CurrentSpawnPoint"))
            {
                if(numSpawnPointsTaken < spawnPoints.Length - 1)
                {
                    int spawnPointRand = Random.Range(0, spawnPoints.Length);
                    while (isSpawnPointTaken[spawnPointRand])
                    {
                        spawnPointRand = Random.Range(0, spawnPoints.Length);
                    }
                    currentPlayerProperties["CurrentSpawnPoint"] = spawnPointRand;
                    isSpawnPointTaken[spawnPointRand] = true;
                    numSpawnPointsTaken++;
                }
                else
                {
                    for(int i = 0; i < spawnPoints.Length; i++)
                    {
                        if (!isSpawnPointTaken[i])
                        {
                            currentPlayerProperties["CurrentSpawnPoint"] = i;
                            break;
                        }
                    }
                } 
            }
            currentPlayer.SetCustomProperties(currentPlayerProperties);
        }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            Player currentPlayer = player.Value;
            currentPlayerProperties = currentPlayer.CustomProperties;
            currentPlayerProperties["shouldSpawn"] = true;
            currentPlayer.SetCustomProperties(currentPlayerProperties);
        }
    }

    public void spawnSelf()
    {
        string chosenChar = chars.getList[(int)PhotonNetwork.LocalPlayer.CustomProperties["SelectedChar"]].name;
        Vector3 spawnPoint = spawnPoints[(int)PhotonNetwork.LocalPlayer.CustomProperties["CurrentSpawnPoint"]].position;
        PhotonNetwork.Instantiate("DuckPrefabs/" + chosenChar, spawnPoint, Quaternion.identity);
    }
}
