using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map.Puzzle.TemplePuzzle
{
    [CreateAssetMenu(fileName = "NewTempleTileData", menuName = "ScriptableObject/TempleTileData")]
    public class TempleTileData : ScriptableObject
    {
        public bool isRotatable = false;
        public bool up = false;
        public bool down = false;
        public bool left = false;
        public bool right = false;
    }
}