using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundarySetter : MonoBehaviour
{
    [SerializeField] private List<Vector2> boundaryPoints;
    
    public void Start()
    {
        CameraController.BoundaryPoints = boundaryPoints;
    }
}
