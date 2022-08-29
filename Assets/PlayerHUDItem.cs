using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerHUDItem : MonoBehaviour
{
    public TMP_Text playerName;
    public CharacterList characterList;
    public Player owner;
    public PlayerController controller;
    public PlayerManager pManager;

    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

    [Header("Icon")]
    public Image charIcon;
    public Color32 playerDeadColor;

    [Header("Lives")]
    public GameObject lifePrefab;
    public Transform LivesTransform;
    public List<GameObject> lives = new List<GameObject>();

    public int numLives;

    // Start is called before the first frame update
    void Start()
    {
        pManager = FindObjectOfType<PlayerManager>();
        
        pManager.PlayerDied += LoseLife;
        numLives = 30;
        if (PhotonNetwork.IsConnected)
        {
            playerProperties = owner.CustomProperties;
            playerName.text = owner.NickName;
            numLives = (int)PhotonNetwork.CurrentRoom.CustomProperties["LifeCount"];
            charIcon.sprite = characterList.getList[(int)owner.CustomProperties["SelectedChar"]].CSS;
        }

        updateLifeCount();   
    }

    private void Update()
    {
        if (!PhotonNetwork.IsConnected)
        {

        }
    }

    void updateLifeCount()
    {
        foreach (GameObject life in lives)
        {
            Destroy(life.gameObject);
        }
        lives.Clear();

        for (int i = 0; i < numLives; i++)
        {
            GameObject life = Instantiate(lifePrefab, LivesTransform);
            lives.Add(life);
        }
        Debug.Log("HUD lives refreshed");
    }


    void LoseLife(Player player, PlayerController controller)
    {
        numLives = controller.lives;
        Debug.Log(numLives);
        if (player == this.owner && numLives != lives.Count)
        {
            if(numLives == 0)
            {
                charIcon.color = playerDeadColor;
            }
            updateLifeCount();
        }
    }
}
