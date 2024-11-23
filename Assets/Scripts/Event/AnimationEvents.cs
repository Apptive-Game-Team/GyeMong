using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AnimationEvent : Event {
    [SerializeField]
    private List<Sprite> frames;
    [SerializeField]
    private float deltaTime = 0.1f;

    public override IEnumerator execute(EventObject eventObject = null)
    {
        SpriteRenderer renderer = eventObject.GetComponent<SpriteRenderer>();
        foreach(Sprite sprite in frames)
        {
            yield return new WaitForSeconds(deltaTime);
            renderer.sprite = sprite;           
        }
    }
}