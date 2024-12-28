using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject player;
    private int defaultCameraZ = -10;

    private const float SHAKE_AMOUNT = 0.1f;
    private const float SHAKE_DELAY = 0.1f;
    private bool isShaking = false;
    private bool isFollowing = true;

    private static Vector2[] polygon;
    public static List<Vector2> BoundaryPoints
    {
        set
        {
            polygon = value.ToArray();
        }
    }
    
    public bool IsFollowing
    {
        get => isFollowing && !isShaking;
        set => isFollowing = value;
    }
    
    void Awake()
    {
        EffectManager.Instance.SetCameraController(this);
        player = GameObject.Find("Player");
    }

    
    void Update()
    {
        if (IsFollowing)
            UpdateCameraPosition();

    }
    
    Vector2 FindClosestPoint(Vector3 point, Vector2[] polygon)
    {
        float minDistance = float.MaxValue;
        Vector2 closestPoint = point;

        for (int i = 0; i < polygon.Length; i++)
        {
            Vector2 p1 = polygon[i];
            Vector2 p2 = polygon[(i + 1) % polygon.Length];

            Vector2 projectedPoint = ProjectPointOnLineSegment(point, p1, p2);
            float distance = Vector2.Distance(point, projectedPoint);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = projectedPoint;
            }
        }

        return closestPoint;
    }

    Vector2 ProjectPointOnLineSegment(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
    {
        Vector2 line = lineEnd - lineStart;
        float t = Mathf.Clamp01(Vector2.Dot(point - lineStart, line) / line.sqrMagnitude);
        return lineStart + t * line;
    }

    
    bool IsPointInsidePolygon(Vector2 point, Vector2[] polygon)
    {
        int intersectCount = 0;
        for (int i = 0; i < polygon.Length; i++)
        {
            Vector2 p1 = polygon[i];
            Vector2 p2 = polygon[(i + 1) % polygon.Length];

            if ((point.y > Mathf.Min(p1.y, p2.y) && point.y <= Mathf.Max(p1.y, p2.y)) &&
                (point.x <= Mathf.Max(p1.x, p2.x)))
            {
                float xIntersect = (point.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x;
                if (p1.y != p2.y && point.x <= xIntersect)
                {
                    intersectCount++;
                }
            }
        }
        return (intersectCount % 2) != 0;
    }

    private void UpdateCameraPosition()
    {
        if (IsPointInsidePolygon(player.transform.position, polygon))
        {
            transform.position = player.transform.position + Vector3.forward * defaultCameraZ;
        }
        else
        {
            Vector2 closestPoint = FindClosestPoint(player.transform.position, polygon);
            Vector3 closestPoint3D = new Vector3(closestPoint.x, closestPoint.y, defaultCameraZ);
            transform.position = closestPoint3D;
        }
        
    }

    public IEnumerator MoveTo(Vector3 target, float duration, float size)
    {
        float timer = 0;
        Vector3 startPosition = transform.position;
        
        Camera camera = GetComponent<Camera>();
        float startSize = camera.orthographicSize;
        while (timer < duration)
        {
            yield return new WaitForSeconds(0.02f);
            timer += 0.02f;
            transform.position = Vector3.Lerp(startPosition, target, timer / duration);
            camera.orthographicSize = Mathf.Lerp(startSize, size, timer / duration);
        }
    }
    
    public IEnumerator ShakeCamera(float time)
    {
        float timer = 0;
        isShaking = true;
        while (timer < time)
        {
            yield return new WaitForSeconds(SHAKE_DELAY);
            timer += 0.1f;
            transform.position = player.transform.position + Random.insideUnitSphere * SHAKE_AMOUNT + Vector3.forward * defaultCameraZ;
        }
        isShaking = false;
    }
}
