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
        playerName.text = owner.NickName;
        charIcon.sprite = characterList.getList[(int)owner.CustomProperties["SelectedChar"]].CSS;
        pManager.PlayerDied += LoseLife;

        playerProperties = owner.CustomProperties;

        numLives = (int)owner.CustomProperties["Lives"];

        updateLifeCount();   
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
        numLives = (int)player.CustomProperties["Lives"];
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
