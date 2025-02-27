using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTempleTileData", menuName = "ScriptableObject/TempleTileData")]
public class TempleTileData : ScriptableObject
{
    public bool up, down, left, right;
}