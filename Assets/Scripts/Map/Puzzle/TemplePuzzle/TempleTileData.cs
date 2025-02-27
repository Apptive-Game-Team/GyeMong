using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleTileData
{
    public bool up, down, left, right;

    public TempleTileData(bool up, bool down, bool left, bool right)
    {
        this.up = up;
        this.down = down;
        this.left = left;
        this.right = right;
    }
}