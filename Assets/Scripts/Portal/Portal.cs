using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Portal : MonoBehaviour
{
    [SerializeField] PortalID portalID;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(PortalManager.Instance.TransitScene(portalID));
    }
}
