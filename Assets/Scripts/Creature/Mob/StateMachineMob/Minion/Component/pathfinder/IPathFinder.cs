using System.Collections.Generic;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public interface IPathFinder
{
    public List<Vector2> FindPath(Vector2 start, Vector2 destination);
    public List<Vector2> FindPath(Vector2 start);
    public List<Vector2> FindPathAvoiding(Vector2 start, Vector2 avoidedPosition);
}
