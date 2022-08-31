using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio List", menuName = "Duck Royale/Audio List", order = 3)]
public class AudioList : ScriptableObject
{
    [SerializeField] private List<AudioClip> list;
    public List<AudioClip> getList => list;
}
