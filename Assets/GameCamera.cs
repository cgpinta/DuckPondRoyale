using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public Vector2 startLocation;
    public Vector2 cameraLimits;


    List<GameObject> players = new List<GameObject>();
    PlayerManager pManager;

    float minX;
    float maxX;
    float minY;
    float maxY;

    bool isGameStarted;

    // Start is called before the first frame update
    void Start()
    {
        pManager = FindObjectOfType<PlayerManager>();

        transform.position = new Vector3(startLocation.x, startLocation.y, -10);
        isGameStarted = false;

        if (pManager != null)
        {
            pManager.GameStart += LoadCameraDetails;
        }
        else
        {
            LoadCameraDetails();
        }
        
    }
    private void Update()
    {
        if (!isGameStarted) { return; }
        if (players.Count == 0){ return; }

        Vector2 centerPoint = GetCenterPoint();
        transform.position = new Vector3(centerPoint.x, centerPoint.y, -10);


    }

    void LoadCameraDetails()
    {
        players = GameObject.FindGameObjectsWithTag("Player").ToList<GameObject>();
        isGameStarted = true;
    }


    Vector2 GetCenterPoint()
    {
        minX = Mathf.Infinity; maxX = -Mathf.Infinity; minY = Mathf.Infinity; maxY = -Mathf.Infinity;
        
        //if(players)
        Bounds bounds = new Bounds();

        foreach (GameObject player in players)
        {
            if(player == null)
            {
                continue;
            }
            bounds.Encapsulate(player.transform.position);
        }

        return bounds.center;
    }

}
