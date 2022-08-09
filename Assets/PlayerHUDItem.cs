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
        charIcon.sprite = characterList.getList[(int)owner.CustomProperties["selectedChar"]].CSS;
        pManager.PlayerDied += LoseLife;

        for(int i = 0; i < numLives; i++)
        {
            GameObject life = Instantiate(lifePrefab, LivesTransform);
            lives.Add(life);
        }
    }


    void LoseLife(Player player)
    {
        if(player == this.owner)
        {
            if(numLives > 0)
            {
                Destroy(lives[0]);
                lives.RemoveAt(0);
                numLives--;
            }
            if(numLives == 0)
            {
                charIcon.color = playerDeadColor;
            }
        }
    }

    
}
