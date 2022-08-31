using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using ExitGames.Client.Photon;

public class PlayerManager : MonoBehaviour
{
    public StageSettings curStageSettings;

    public CharacterList chars;
    public GameObject spawnpointParent;

    private const byte SpawnPlayers = 1;

    [Header("Stage UI")]
    public TMP_Text countdownTextBox;
    public GameObject PlayerHUDPrefab;
    public Transform PlayerHUDTransform;
    public Canvas PlayerHUDCanvas;

    [Header("Win Screen")]
    public GameObject WinScreen;
    public AudioClip buttonClick;
    

    [Header("Sounds")]
    AudioSource audioSource;
    public AudioList hitSounds;




    bool playersLoaded = false;

    int lifeCount = 3;

    Timer countdownTimer = new Timer(true);
    Animator countdownAnimator;
    List<string> countdownText = new List<string>();
    int countdownNextNumber = 0;
    bool countingDown;

    List<Transform> spawnPoints = new List<Transform>();
    Player localPlayer;              //the local player
    ExitGames.Client.Photon.Hashtable currentPlayerProperties = new ExitGames.Client.Photon.Hashtable();

    GameObject localPlayerObject;

    Dictionary<int, Player> playerList = new Dictionary<int, Player>();
    PlayerController[] controllerList;


    #region Variables for player spawning
    bool[] isSpawnPointTaken;
    int numSpawnPointsTaken = 0;
    List<Player> loadedPlayerList = new List<Player>();
    #endregion

    public event Action<Player> PlayerLoaded;
    public event Action GameStart;
    public Action<Player, PlayerController> PlayerDied;
    public Action<Player, PlayerController> PlayerRespawned;
    public Action<PlayerController> PlayerWon;

    private void Start()
    {
        Debug.Log("Stage loaded");
        audioSource = GetComponent<AudioSource>();
        Camera cam = Camera.main;
        PlayerHUDCanvas.worldCamera = cam;
        WinScreen.SetActive(false);

        countdownText.Add("Goose");
        countdownText.Add("Goose");
        countdownText.Add("DUCK!");
        countdownAnimator = countdownTextBox.gameObject.GetComponent<Animator>();

        PlayerDied += UpdatePlayerAfterDeath;

        countdownTextBox.gameObject.SetActive(false);
        countingDown = false;

        spawnPoints = spawnpointParent.GetComponentsInChildren<Transform>().ToList();
        spawnPoints.RemoveAt(0);
        isSpawnPointTaken = new bool[spawnPoints.Count];

        if (PhotonNetwork.IsConnected)
        {
            localPlayer = PhotonNetwork.LocalPlayer;
            

            currentPlayerProperties["HasLoadedStage"] = true;
            //currentPlayerProperties["Lives"] = lifeCount;
            localPlayer.SetCustomProperties(currentPlayerProperties);
            SetPlayerSpawnpoint(localPlayer);
        }
        else
        {
            StartCountdown(); //test script
            controllerList = FindObjectsOfType<PlayerController>();
            foreach(PlayerController controller in controllerList)
            {
                controller.lives = 30;
            }
            OfflineLoadHUD();
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
                    if (PhotonNetwork.IsConnected)
                    {
                        LoadHUD();
                    }
                    
                    GameStart();
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

    void OfflineLoadHUD()
    {
        controllerList = FindObjectsOfType<PlayerController>();
        foreach (PlayerController controller in controllerList)
        {
            GameObject HUD = Instantiate(PlayerHUDPrefab, PlayerHUDTransform);
            PlayerHUDItem HUDitem = HUD.GetComponent<PlayerHUDItem>();
            HUDitem.controller = controller;
            //HUDitem.owner = player.Value;

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
            if (numSpawnPointsTaken < spawnPoints.Count - 1)
            {
                int spawnPointRand = Random.Range(0, spawnPoints.Count);
                while (isSpawnPointTaken[spawnPointRand])
                {
                    spawnPointRand = Random.Range(0, spawnPoints.Count);
                }
                currentPlayerProperties["CurrentSpawnPoint"] = spawnPointRand;
                isSpawnPointTaken[spawnPointRand] = true;
                numSpawnPointsTaken++;
            }
            else
            {
                for (int i = 0; i < spawnPoints.Count; i++)
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
        PlayerController curController = currentPlayerObj.GetComponent<PlayerController>();
        curController.lives = lifeCount;
        curController.player = PhotonNetwork.LocalPlayer;


        localPlayerObject = currentPlayerObj;
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


    public void UpdatePlayerAfterDeath(Player player, PlayerController controller)
    {

        controller.lives--;
        Debug.Log("New Lives:"+controller.lives);
        if(controller.lives > 0)
        {
            if (PhotonNetwork.IsConnected)
            {
                controller.Respawn(spawnPoints[0].position);
                PlayerRespawned(player, controller);
                //controller.gameObject.SetActive(true);
            }
            else
            {
                controller.Respawn(spawnPoints[0].position);
                PlayerRespawned(player, controller);
            }
            //controller.Respawn(spawnPoints[0].position);
        }
        else
        {
            //TODO: PUNRPC METHOD FOR SHOWING WIN SCREEN
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(controller.gameObject);
            }
            
            PlayerController winner = AreAllPlayersDead();
            if (winner != null)
            {
                //PhotonNetwork.RaiseEvent(2, null, RaiseEventOptions.Default, SendOptions.SendUnreliable);
                //PlayerWon(winner);
                controllerList = FindObjectsOfType<PlayerController>();
                
                if(PhotonNetwork.LocalPlayer == winner.player)
                {
                    PhotonNetwork.Destroy(winner.gameObject);
                }

                WinScreen.SetActive(true);
                WinScreen.GetComponent<WinScreen>().DisplayWinDetails(winner);
            }
        }
        PhotonNetwork.SetPlayerCustomProperties(currentPlayerProperties);
    }


    public PlayerController AreAllPlayersDead()
    {
        PlayerController winner = null;
        int numOfPlayersAlive = 0;

        if(controllerList == null)
        {
            controllerList = FindObjectsOfType<PlayerController>();
        }

        foreach (PlayerController controller in controllerList)
        {
            //if ((int)player.Value.CustomProperties["Lives"] > 0)
            if(controller.lives > 0)
            {
                numOfPlayersAlive++;
                winner = controller;
            }
            if(numOfPlayersAlive > 1)
            {
                return null;
            }
        }
        return winner;
    }

    public void ResetPlayerProperties()
    {
        //foreach(KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        //{
        //    currentPlayerProperties = player.Value.CustomProperties;
        //    if (currentPlayerProperties.ContainsKey("Winner")) { currentPlayerProperties.Remove("Winner"); }
        //    PhotonNetwork.SetPlayerCustomProperties(currentPlayerProperties);
        //}
    }

    public void OnClickReturnToRoom()
    {
        PlaySound(buttonClick);
        ResetPlayerProperties();
        PhotonNetwork.LoadLevel("Lobby");
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play(0);
    }
}
