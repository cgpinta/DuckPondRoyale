using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;
using Random = UnityEngine.Random;

public class PlayerSpawner : MonoBehaviour
{
    public CharacterList chars;

    [Header("Stage UI")]
    public TMP_Text countdownTextBox;


    Timer countdownTimer = new Timer(true);
    Animator countdownAnimator;
    List<string> countdownText = new List<string>();
    int countdownNextNumber = 0;
    bool countingDown;

    Transform[] spawnPoints;
    Player localPlayer;              //the local player
    ExitGames.Client.Photon.Hashtable currentPlayerProperties = new ExitGames.Client.Photon.Hashtable();


    

    #region Variables for player spawning
    bool[] isSpawnPointTaken;
    int numSpawnPointsTaken = 0;
    List<Player> loadedPlayerList = new List<Player>();
    #endregion

    public event Action<Player> PlayerLoaded;
    public event Action SpawnPlayers;

    private void Start()
    {
        countdownText.Add("Duck");
        countdownText.Add("Duck");
        countdownText.Add("GOOSE!");
        countdownAnimator = countdownTextBox.gameObject.GetComponent<Animator>();


        PlayerLoaded += CheckIfAllPlayersLoaded;
        SpawnPlayers += SpawnSelf;
        countdownTextBox.gameObject.SetActive(false);
        countingDown = false;
        if (PhotonNetwork.IsConnected)
        {
            localPlayer = PhotonNetwork.LocalPlayer;
            spawnPoints = GetComponentsInChildren<Transform>();
            isSpawnPointTaken = new bool[spawnPoints.Length];

            currentPlayerProperties["HasLoadedStage"] = true;
            localPlayer.SetCustomProperties(currentPlayerProperties);
            PlayerLoaded(localPlayer);

        }
        else
        {
            StartCountdown(); //test script
        }
    }

    public event Action IntroCountdownEnded;
    private void FixedUpdate()
    {
        if (countingDown)
        {
            if (countdownTimer.getWatch() > countdownNextNumber)
            {
                if (countdownNextNumber == countdownText.Count)
                {
                    countdownTextBox.gameObject.SetActive(false);
                    countingDown = false;
                }
                else if(countdownNextNumber + 1 == countdownText.Count)
                {
                    countdownNextNumber++;
                    countdownTextBox.text = countdownText[countdownNextNumber - 1];
                    Debug.Log(countdownTextBox.text);
                    countdownAnimator.SetTrigger("CountFinal");
                }
                else
                {
                    countdownNextNumber++;
                    countdownTextBox.text = countdownText[countdownNextNumber - 1];
                    countdownAnimator.SetTrigger("Count");
                }
            }
        }
    }

    private void CheckIfAllPlayersLoaded(Player player)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            loadedPlayerList.Add(player);
            
            currentPlayerProperties = player.CustomProperties;

            if (!player.CustomProperties.ContainsKey("CurrentSpawnPoint"))
            {
                if (numSpawnPointsTaken < spawnPoints.Length - 1)
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
                    for (int i = 0; i < spawnPoints.Length; i++)
                    {
                        if (!isSpawnPointTaken[i])
                        {
                            currentPlayerProperties["CurrentSpawnPoint"] = i;
                            break;
                        }
                    }
                }
            }
            player.SetCustomProperties(currentPlayerProperties);

            if (loadedPlayerList.Count == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                SpawnPlayers();
            }
        }
    }

    private void SpawnSelf()
    {
        string chosenChar = chars.getList[(int)PhotonNetwork.LocalPlayer.CustomProperties["SelectedChar"]].name;
        Vector3 spawnPoint = spawnPoints[(int)PhotonNetwork.LocalPlayer.CustomProperties["CurrentSpawnPoint"]].position;
        PhotonNetwork.Instantiate("DuckPrefabs/" + chosenChar, spawnPoint, Quaternion.identity);
        StartCountdown();
    }

    public void StartCountdown()
    {
        countingDown = true;
        countdownTextBox.text = countdownText[0];
        countdownNextNumber = 1;
        countdownTextBox.gameObject.SetActive(true);
        countdownTimer.startWatch("countdown");
        countdownAnimator.SetTrigger("Count");
    }
}
