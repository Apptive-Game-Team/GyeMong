using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.EventSystem.Event.CinematicEvent
{
    [Serializable]
    public class AnimationEvent : Event {
        [SerializeField]
        private List<Sprite> frames;
        [SerializeField]
        private float deltaTime = 0.1f;

        [SerializeField] private SpriteRenderer _renderer = null;

        public void SetRenderer(SpriteRenderer renderer)
        {
            _renderer = renderer;
        }

        public void SetFrames(List<Sprite> spriteList)
        {
            frames = spriteList;
        }

        public void SetDeltaTime(float time)
        {
            deltaTime = time;
        }

        public override IEnumerator Execute(EventObject eventObject = null)
        {
            if (_renderer == null) 
                _renderer = eventObject.GetComponent<SpriteRenderer>();
            foreach(Sprite sprite in frames)
            {
                yield return new WaitForSeconds(deltaTime);
                _renderer.sprite = sprite;           
            }
        }
    }
}