using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class InGameUI : MonoBehaviour
{
    Camera mainCamera;

    Canvas canvas;

    [SerializeField] TMP_Text damageText;
    [SerializeField] TMP_Text playerName;
    [SerializeField] PlayerController pController;
    [SerializeField] PhotonView pView;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();
        mainCamera = FindObjectOfType<Camera>();

        canvas.worldCamera = mainCamera;

        playerName.text = pView.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        if(pController != null)
            damageText.text = pController.damage.ToString();
    }
}
