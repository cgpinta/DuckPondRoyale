using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "StageSettings", menuName = "Duck Royale/New Stage Settings")]
[Serializable]
public class StageSettings : ScriptableObject
{
    [SerializeField] private int id;
    public int ID => id;


    [Header("Asthetics")]
    //[SerializeField] 
    [SerializeField] string sceneName;
    public string SceneName => sceneName;
    [SerializeField] Sprite sss;                                //used in CSS
    public Sprite SSS => sss;
}
