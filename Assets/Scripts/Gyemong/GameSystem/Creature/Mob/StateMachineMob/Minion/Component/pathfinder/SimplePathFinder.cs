using System.Collections.Generic;
using UnityEngine;

namespace Gyemong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.pathfinder
{
    public class SimplePathFinder : IPathFinder
    {
        public List<Vector2> FindPath(Vector2 start, Vector2 destination)
        {
            return new List<Vector2> { destination };
        }

        public List<Vector2> FindPath(Vector2 start)
        {
            return new List<Vector2>();
        }

        public List<Vector2> FindPathAvoiding(Vector2 start, Vector2 avoidedPosition)
        {
            return new List<Vector2> { start * 2 - avoidedPosition };
        }
    }
}