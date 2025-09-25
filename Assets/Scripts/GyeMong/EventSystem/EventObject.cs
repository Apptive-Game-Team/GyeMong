using System.Collections;
using System.Collections.Generic;
using GyeMong.EventSystem.Controller.Condition;
using GyeMong.EventSystem.Interface;
using GyeMong.GameSystem.Interface;
using GyeMong.GameSystem.Object;
using UnityEngine;

namespace GyeMong.EventSystem
{
    public class EventObject : InteractableObject, IAttackable, IEventTriggerable
    {
        public enum EventTrigger
        {
            OnCollisionEnter = 0,
            OnInteraction = 1,
            OnAwake = 2,
            OnAttacked = 3,
            OnCalled = 4,
            OnEnabled = 5,
        }

        [SerializeField]
        private bool isLoop = false;

        [SerializeField]
        public EventTrigger trigger;

        [SerializeField]
        private int triggerLimitCounter = -1;

        [SerializeReference]
        private List<Event.Event> eventSequence = new List<Event.Event>();

        private Coroutine eventLoop = null;

        public Event.Event[] EventSequence => eventSequence.ToArray(); 
    
        public void SetTriggerLimitCounter(int counter)
        {
            triggerLimitCounter = counter;
        }
        
        private void Start()
        {
            if (trigger == EventTrigger.OnAwake && triggerLimitCounter != 0)
            {
                TriggerEvent();
                triggerLimitCounter -= 1;
            }
        }

        private void OnEnable()
        {
            if (trigger == EventTrigger.OnEnabled && triggerLimitCounter != 0)
            {
                TriggerEvent();
                triggerLimitCounter -= 1;
            }
        }
    
        private void OnDisable()
        {
            KillEvent();
        }
    
        private IEnumerator EventLoop()
        {
            do
            {
                foreach (Event.Event eventObject in eventSequence)
                {
                    yield return eventObject.Execute(this);
                }
            } while (isLoop);
            eventLoop = null;
        }

        public void Trigger()
        {
            if (triggerLimitCounter != 0)
            {
                TriggerEvent();
                triggerLimitCounter -= 1;
            }
        }
    
        private void TriggerEvent()
        {
            if (eventLoop != null)
            {
                return;
            }
            eventLoop = StartCoroutine(EventLoop());
        }

        public void KillEvent()
        {
            if (eventLoop != null)
                StopCoroutine(eventLoop);
            eventLoop = null;
        }

        protected override void OnInteraction(Collider2D collision)
        {
            if (trigger == EventTrigger.OnInteraction && triggerLimitCounter != 0)
            {
                TriggerEvent();
                triggerLimitCounter -= 1;
            }
            
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (trigger == EventTrigger.OnCollisionEnter && collision.CompareTag("Player") && triggerLimitCounter != 0)
            {
                TriggerEvent();
                triggerLimitCounter -= 1;
            }
        }

        public void OnAttacked(float damage = 0)
        {
            if (trigger == EventTrigger.OnAttacked && triggerLimitCounter != 0)
            {
                TriggerEvent();
                triggerLimitCounter -= 1;
            }
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}
