using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character List", menuName = "Duck Royale/Character List", order = 0)]
public class CharacterList : ScriptableObject
{
    [SerializeField] private List<DuckSettings> list;
    public List<DuckSettings> getList => list;
}
