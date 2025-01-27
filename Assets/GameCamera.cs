using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public enum cameraType
{
    SmashCam,
    FollowPlayer1,
    Fixed
}

public class GameCamera : MonoBehaviour
{
    StageSettings stageSettings;

    public Vector2 startLocation;
    public Vector2 cameraLimits;
    public cameraType camType;
    public Vector2 fixedLocation;
    public float zoom = 12;
    float defaultZoom = 12;
    public float smoothTime;

    Camera cam;

    List<GameObject> players = new List<GameObject>();
    PlayerManager pManager;

    float minX;
    float maxX;
    float minY;
    float maxY;

    bool isGameStarted;
    int depth = -10;

    Vector3 velocity;


    float horzExtent;
    float vertExtent;

    // Start is called before the first frame update
    void Start()
    {
        pManager = FindObjectOfType<PlayerManager>();
        stageSettings = pManager.curStageSettings;

        cameraLimits = stageSettings.CameraLimits;

        cam = Camera.main;

        vertExtent = cam.orthographicSize;
        horzExtent = vertExtent * Screen.width/Screen.height;

        


        transform.position = new Vector3(startLocation.x, startLocation.y, -10);
        isGameStarted = false;

        if (pManager != null)
        {
            pManager.GameStart += LoadCameraDetails;
        }
        if (!PhotonNetwork.IsConnected)
        {
            LoadCameraDetails();
        }
        
    }
    private void LateUpdate()
    {
        if (!isGameStarted) { return; }
        if (players.Count == 0){ return; }
        
        if(camType == cameraType.SmashCam)
        {
            transform.position = Vector3.SmoothDamp(transform.position, KeepCameraInBounds(GetCenterPoint(players)), ref velocity, smoothTime);
            //transform.position = KeepCameraInBounds(GetCenterPoint(players));
        }
        else if(camType == cameraType.FollowPlayer1)
        {
            transform.position = KeepCameraInBounds(GetCenterPoint(players[0]));
        }
        else if(camType == cameraType.Fixed)
        {
            transform.position = new Vector3(fixedLocation.x, fixedLocation.y, depth);
            cam.orthographicSize = zoom;
        }
    }

    private void OnValidate()
    {
        cam = GetComponent<Camera>();
        if(camType == cameraType.Fixed)
        {
            transform.position = new Vector3(fixedLocation.x, fixedLocation.y, depth);
            cam.orthographicSize = zoom;
        }
        else if (camType == cameraType.SmashCam)
        {
            transform.position = new Vector3(startLocation.x, startLocation.y, depth);
            cam.orthographicSize = defaultZoom;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector2.zero, cameraLimits);
    }

    void LoadCameraDetails()
    {
        players = GameObject.FindGameObjectsWithTag("Player").ToList<GameObject>();
        isGameStarted = true;
    }

    Vector3 GetCenterPoint(GameObject focus)
    {
        minX = Mathf.Infinity; maxX = -Mathf.Infinity; minY = Mathf.Infinity; maxY = -Mathf.Infinity;

        if (focus == null)
        {
            return new Vector3(startLocation.x, startLocation.y, depth);
        }

        Bounds bounds = new Bounds(focus.transform.position, Vector3.zero);

        return new Vector3(bounds.center.x, bounds.center.y, depth);
    }

    Vector3 GetCenterPoint(List<GameObject> focuses)
    {
        minX = Mathf.Infinity; maxX = -Mathf.Infinity; minY = Mathf.Infinity; maxY = -Mathf.Infinity;
        
        //if(players)
        Bounds bounds = new Bounds();

        foreach (GameObject focus in focuses)
        {
            if(focus == null)
            {
                continue;
            }
            bounds.Encapsulate(focus.transform.position);
        }

        return new Vector3(bounds.center.x, bounds.center.y, depth);
    }

    Vector3 KeepCameraInBounds(Vector3 currentPos)
    {
        minX = horzExtent - cameraLimits.x / 2f;
        maxX = cameraLimits.x / 2f - horzExtent;
        minY = vertExtent - cameraLimits.y / 2f;
        maxY = cameraLimits.y / 2f - vertExtent;

        Vector3 result = new Vector3(Mathf.Clamp(currentPos.x, minX, maxX),
                    Mathf.Clamp(currentPos.y, minY, maxY),
                    depth);
        return result;
    }

}
