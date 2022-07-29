using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSSBoard : MonoBehaviour
{
    
    public CSSIcon CSSIconPrefab;
    public LobbyManager LobbyManager;

    // Start is called before the first frame update
    void Start()
    {
        LobbyManager = FindObjectOfType<LobbyManager>();

        foreach(DuckSettings character in LobbyManager.GetCharacters())
        {
            CSSIcon newIcon = Instantiate(CSSIconPrefab, this.transform);
            newIcon.settings = character;
            newIcon.GetPropertiesFromSettings();
        }
    }


}
