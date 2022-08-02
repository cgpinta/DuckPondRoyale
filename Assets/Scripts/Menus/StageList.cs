using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage List", menuName = "Duck Royale/Stage List", order = 0)]
public class StageList : ScriptableObject
{
    [SerializeField] private List<StageSettings> list;
    public List<StageSettings> getList => list;
}
