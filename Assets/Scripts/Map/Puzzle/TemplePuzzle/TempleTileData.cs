using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTempleTileData", menuName = "ScriptableObject/TempleTileData")]
public class TempleTileData : ScriptableObject
{
    public bool up = true;
    public bool down = false;
    public bool left = false;
    public bool right = false;
}
