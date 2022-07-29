using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSSBoard : MonoBehaviour
{
    
    public CSSIcon CSSIconPrefab;
    public List<DuckSettings> characters;

    // Start is called before the first frame update
    void Start()
    {
        characters = FindObjectOfType<LobbyManager>().characterList.getList;

        foreach(DuckSettings character in characters)
        {
            CSSIcon newIcon = Instantiate(CSSIconPrefab, this.transform);
            newIcon.settings = character;
            newIcon.GetPropertiesFromSettings();
        }
    }


}
