using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneObject : MonoBehaviour
{
    RuneData runeData;
    SpriteRenderer spriteRenderer;
    EventObject eventObject;

    public void SetRuneData(RuneData newData)
    {
        runeData = newData;
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}
