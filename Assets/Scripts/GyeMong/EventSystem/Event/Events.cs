using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GyeMong.EventSystem.Controller.Condition;
using GyeMong.EventSystem.Interface;
using GyeMong.GameSystem.Creature.Player.Component;
using GyeMong.GameSystem.Map.Portal;
using GyeMong.InputSystem;
using GyeMong.SoundSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GyeMong.EventSystem.Event
{
    [Serializable]
    public abstract class Event
    {
        public abstract IEnumerator Execute(EventObject eventObject = null);

        public virtual Event[] GetChildren()
        {
            return null;
        }

        public Event Find(Type type)
        {
            Event[] children = GetChildren();
            if (children != null)
            {
                foreach (Event child in children)
                {
                    if (child.GetType() == type)
                    {
                        return child;
                    }
                }
            }

            return null;
        }
        public virtual List<ToggeableCondition> FindToggleableConditions()
        {
            return null;
        }
    }

    [Serializable]
    public class WarpPortalEvent : Event
    {
        public PortalID portalID;
        public float delay;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            return PortalManager.Instance.TransitScene(portalID, delay);
        }
    }

    [Serializable]
    public class DelayEvent : Event
    {
        public float delayTime;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            yield return new WaitForSeconds(delayTime);
        }
    }

    [Serializable]
    public class SkippableDelayEvent : Event
    {
        public float delayTime = 10;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            float timer = Time.time;
            yield return new WaitUntil(() =>
            {
                return (timer + delayTime < Time.time) || InputManager.Instance.GetKeyDown(ActionCode.Interaction);
            });
        }
    }

    [Serializable]
    public class SoundEvent : Event
    {
        [SerializeField]
        private SoundObject soundObject;

        private enum PlayOrStop
        {
            PLAY,
            STOP
        }

        [SerializeField]
        private PlayOrStop playOrStop = PlayOrStop.PLAY;

        [SerializeField]
        private string soundName = null;

        [SerializeField]
        private bool sync = true;

        // ReSharper disable Unity.PerformanceAnalysis
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            try
            {
                if (soundObject == null)
                {

                    soundObject = eventObject.GetComponent<SoundObject>();

                }

                if (playOrStop == PlayOrStop.STOP)
                {
                    soundObject.Stop();
                    return null;
                }
                else
                {
                    if (soundName != null)
                        soundObject.SetSoundSourceByName(soundName);
                    if (sync)
                        return soundObject.Play();
                    else
                        soundObject.StartCoroutine(soundObject.Play());
                    return null;
                }
            }
            catch (NullReferenceException)
            {
                soundObject = eventObject.AddComponent<SoundObject>();
                return Execute(eventObject);
            }
        }
    }

    [Serializable]
    public class BGMEvent : Event
    {
        [SerializeField]
        private string bgmName;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            SoundObject soundObject = SoundManager.Instance.GetBgmObject();
            SoundManager.Instance.soundObjects.Add(soundObject);
            soundObject.SetSoundSourceByName(bgmName);
            return soundObject.Play();
        }
    }

    [Serializable]
    public class StopEvent : Event
    {
        [SerializeField]
        private EventObject targetObject;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            targetObject.KillEvent();
            yield return null;
        }
    }

    [Serializable]
    public class MovePositionEvent : Event
    {
        [SerializeField]
        private GameObject @gameObject;
        [SerializeField]
        private Vector3 targetPosition;

        public override IEnumerator Execute(EventObject eventObject = null)
        {
            @gameObject.transform.position = targetPosition;
            yield return null;
        }
    }

    [Serializable]
    public class DestroySelfEvent : Event
    {
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            eventObject.DestroySelf();
            return null;
        }
    }

    [Serializable]
    public class TriggerEvent : Event
    {
        [SerializeField]
        private MonoBehaviour triggerable;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            try
            {
                IEventTriggerable triggerable = this.triggerable as IEventTriggerable;
                triggerable.Trigger();
            } catch (NullReferenceException)
            {
                Debug.LogError("Triggerable is not set");
            }
        
            return null;
        }
    }

    public class EnqueueEvent : Event
    {
        [SerializeReference]
        private Event e;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            EventQueue.Instance.AddEvent(e);
            yield return null;
        }
    }

    public class SetActiveObject : Event
    {
        [SerializeField] public GameObject _gameObject;
        [SerializeField] public bool isActive;

        public override IEnumerator Execute(EventObject eventObject = null)
        {
            _gameObject.SetActive(isActive);
            yield return null;
        }
    }

    public class DropObjectEvent : Event
    {
        [SerializeField] private GameObject _gameObject;

        public override IEnumerator Execute(EventObject eventObject = null)
        {
            GameObject.Instantiate(_gameObject, eventObject.transform.position, _gameObject.transform.rotation);
            _gameObject.transform.DOMoveY(_gameObject.transform.position.y + 1f, 0.3f).SetEase(Ease.OutQuad)
                .OnComplete(() => _gameObject.transform.DOMoveY(_gameObject.transform.position.y, 0.3f).SetEase(Ease.InBounce));
            yield return null;
        }
    }

    public class LoadScene : Event
    {
        [SerializeField] private string _sceneName;

        public override IEnumerator Execute(EventObject eventObject = null)
        {
            SceneManager.LoadScene(_sceneName);
            yield return null;
        }
    }

    public class ChangeObjectColor : Event
    {
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private Color _color;
        public override IEnumerator Execute(EventObject eventObject = null)
        {
            _gameObject.transform.GetComponent<SpriteRenderer>().color = _color;
            yield return null;
        }
    }
}