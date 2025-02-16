using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObject : MonoBehaviour
{
    EffectData effectData;

    public int ID
    {
        get
        {
            return effectData.id;
        }
    }

    private void Start()
    {
        Destroy(gameObject, 0.5f);
    }
}