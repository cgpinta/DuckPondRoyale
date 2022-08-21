using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlanes : MonoBehaviour
{
    StageSettings stageSettings;
    PlayerManager pManager;

    public Vector2 deathLimits;
    float colliderSize = 1;
    public List<GameObject> colliders = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        pManager = FindObjectOfType<PlayerManager>();
        stageSettings = pManager.curStageSettings;

        deathLimits = stageSettings.DeathLimits;

        GameObject deathObject = new GameObject();
        deathObject.tag = "Death";
        deathObject.name = "DeathCollider";
        BoxCollider2D deathCollider = deathObject.AddComponent<BoxCollider2D>();
        deathCollider.isTrigger = true;
        for (int i = 0; i < 4; i++)
        {
            colliders.Add(Instantiate(deathObject, this.transform));
        }

        //set right x death plane
        colliders[0].transform.localScale = new Vector3(colliderSize, deathLimits.y + colliderSize * 2, 0);
        colliders[0].transform.position = new Vector3(deathLimits.x/2f + colliderSize/2f, 0, 0);
        //set left x death plane
        colliders[1].transform.localScale = new Vector3(colliderSize, deathLimits.y + colliderSize * 2, 0);
        colliders[1].transform.position = new Vector3(-deathLimits.x/2f - colliderSize/2f, 0, 0);

        //set top y death plane
        colliders[2].transform.localScale = new Vector3(deathLimits.x + colliderSize * 2, colliderSize, 0);
        colliders[2].transform.position = new Vector3(0, deathLimits.y/2f + colliderSize/2f, 0);
        //set bottom y death plane
        colliders[3].transform.localScale = new Vector3(deathLimits.x + colliderSize*2, colliderSize, 0);
        colliders[3].transform.position = new Vector3(0, -deathLimits.y/2f - colliderSize/2f, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector2.zero, deathLimits);
    }
}
