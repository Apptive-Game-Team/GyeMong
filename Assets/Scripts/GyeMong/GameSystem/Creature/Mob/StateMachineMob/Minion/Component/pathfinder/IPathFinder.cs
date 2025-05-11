using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.pathfinder
{
    public interface IPathFinder
    {
        public List<Vector2> FindPath(Vector2 start, Vector2 destination);
        public List<Vector2> FindPath(Vector2 start);
        public List<Vector2> FindPathAvoiding(Vector2 start, Vector2 avoidedPosition);
    }
}
