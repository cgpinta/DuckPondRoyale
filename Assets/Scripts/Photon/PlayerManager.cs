using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;
using Random = UnityEngine.Random;
using ExitGames.Client.Photon;

public class PlayerManager : MonoBehaviour
{
    public CharacterList chars;


    private const byte SpawnPlayers = 1;

    [Header("Stage UI")]
    public TMP_Text countdownTextBox;
    public GameObject PlayerHUDPrefab;
    public Transform PlayerHUDTransform;

    bool playersLoaded = false;

    int lifeCount = 3;

    Timer countdownTimer = new Timer(true);
    Animator countdownAnimator;
    List<string> countdownText = new List<string>();
    int countdownNextNumber = 0;
    bool countingDown;

    Transform[] spawnPoints;
    Player localPlayer;              //the local player
    ExitGames.Client.Photon.Hashtable currentPlayerProperties = new ExitGames.Client.Photon.Hashtable();

    List<GameObject> playerObjects = new List<GameObject>();

    Dictionary<int, Player> playerList = new Dictionary<int, Player>();

    #region Variables for player spawning
    bool[] isSpawnPointTaken;
    int numSpawnPointsTaken = 0;
    List<Player> loadedPlayerList = new List<Player>();
    #endregion

    public event Action<Player> PlayerLoaded;
    public event Action ActivateAllPlayerInput;
    public Action<Player> PlayerDied;

    private void Start()
    {
        countdownText.Add("Duck");
        countdownText.Add("Duck");
        countdownText.Add("GOOSE!");
        countdownAnimator = countdownTextBox.gameObject.GetComponent<Animator>();

        countdownTextBox.gameObject.SetActive(false);
        countingDown = false;
        if (PhotonNetwork.IsConnected)
        {
            
            localPlayer = PhotonNetwork.LocalPlayer;
            spawnPoints = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>();
            isSpawnPointTaken = new bool[spawnPoints.Length];

            currentPlayerProperties["HasLoadedStage"] = true;
            localPlayer.SetCustomProperties(currentPlayerProperties);
            SetPlayerSpawnpoint(localPlayer);
        }
        else
        {
            StartCountdown(); //test script
        }
    }

    public event Action IntroCountdownEnded;
    private void FixedUpdate()
    {
        if (!playersLoaded)
        {
            playersLoaded = CheckIfAllPlayersLoaded();
            return;
        }
        if (countingDown)
        {
            if (countdownTimer.getWatch() > countdownNextNumber)
            {
                if (countdownNextNumber == countdownText.Count)
                {
                    countdownTextBox.gameObject.SetActive(false);
                    countingDown = false;
                    LoadHUD();
                    ActivateAllPlayerInput();
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

    void LoadHUD()
    {
        playerList = PhotonNetwork.CurrentRoom.Players;
        foreach (KeyValuePair<int, Player> player in playerList)
        {
            GameObject HUD = Instantiate(PlayerHUDPrefab, PlayerHUDTransform);
            PlayerHUDItem HUDitem = HUD.GetComponent<PlayerHUDItem>();
            HUDitem.numLives = lifeCount;
            HUDitem.owner = player.Value;

        }
    }

    private bool CheckIfAllPlayersLoaded()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            loadedPlayerList.Clear();
            foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                if (!player.Value.CustomProperties.ContainsKey("HasLoadedStage"))
                {
                    return false;
                }
            }

            PhotonNetwork.RaiseEvent(SpawnPlayers, null, RaiseEventOptions.Default, SendOptions.SendUnreliable);
            SpawnSelf(new EventData { Code = SpawnPlayers }) ;
            return true;
        }
        return true;
    }


    private void SetPlayerSpawnpoint(Player player)
    {
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
        //currentPlayerProperties["damage"] = ;
        player.SetCustomProperties(currentPlayerProperties);
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += SpawnSelf;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= SpawnSelf;
    }
    private void SpawnSelf(EventData obj)
    {
        if(obj.Code != SpawnPlayers)
        {
            return;
        }
        string chosenChar = chars.getList[(int)PhotonNetwork.LocalPlayer.CustomProperties["SelectedChar"]].name;
        Vector3 spawnPoint = spawnPoints[(int)PhotonNetwork.LocalPlayer.CustomProperties["CurrentSpawnPoint"]].position;
        
        GameObject currentPlayerObj = PhotonNetwork.Instantiate("DuckPrefabs/" + chosenChar, spawnPoint, Quaternion.identity);
        currentPlayerObj.GetComponent<PlayerInput>().DeactivateInput();
        currentPlayerObj.GetComponent<PlayerController>().lives = lifeCount;

        //GameObject HUD = Instantiate(PlayerHUDPrefab, PlayerHUDTransform);
        //PlayerHUDItem HUDitem = HUD.GetComponent<PlayerHUDItem>();
        //HUDitem.numLives = lifeCount;
        //HUDitem.owner = localPlayer;

        playerObjects.Add(currentPlayerObj);
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
