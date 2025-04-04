using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("Use Cinemachine instead")]
public class BoundarySetter : MonoBehaviour
{
    [SerializeField] private List<Vector2> boundaryPoints;
    [SerializeField] private List<Vector2> colliderPoints;
    private EdgeCollider2D edgeCollider;

    public void Start()
    {
        CameraController.BoundaryPoints = boundaryPoints;

        edgeCollider = GetComponent<EdgeCollider2D>();
        SetBoundaryCollider();
    }

    void SetBoundaryCollider()
    {
        if (colliderPoints.Count < 2) return;

        edgeCollider.points = colliderPoints.ToArray();
    }
}
