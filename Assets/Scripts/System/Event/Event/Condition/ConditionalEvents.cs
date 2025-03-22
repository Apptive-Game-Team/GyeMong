using System.Collections;
using System.Collections.Generic;
using System.Event.Controller.Condition;
using UnityEngine;

namespace System.Event.Event.Condition
{
    public abstract class ConditionalEvent : Event
    {
        [SerializeReference]
        protected Controller.Condition.Condition _condition;
        public override List<ToggeableCondition> FindToggleableConditions()
        {
            List<ToggeableCondition> result = new List<ToggeableCondition>();
            Event[] children = GetChildren();
            if (children != null)
            {
                foreach (Event @event in children)
                {
                    List<ToggeableCondition> temp;
                    temp = @event?.FindToggleableConditions();
                    if (temp != null)
                    {
                        result.AddRange(temp);
                    }
                }
            }
        
            if (_condition is ToggeableCondition)
            {
                result.Add((ToggeableCondition) _condition);
            }
            return result;
        }
    }

    [Serializable]
    public class ConditionalBranchEvent : ConditionalEvent
    {
        [SerializeReference]
        private Event eventInTrue;

        [SerializeReference]
        private Event eventInFalse;

        public override IEnumerator Execute(EventObject eventObject = null)
        {
            if (_condition.Check())
            {
                return eventInTrue?.Execute(eventObject);
            } else
            {
                return eventInFalse?.Execute(eventObject);
            }
        }

        public override Event[] GetChildren()
        {
            return new Event[] { eventInTrue, eventInFalse };
        }
    }

    [Serializable]
    public class ConditionalLoopEvent : ConditionalEvent
    {
        [SerializeReference]
        private Event loopBodyevent;

        public override IEnumerator Execute(EventObject eventObject = null)
        {
            while (_condition.Check())
            {
                yield return loopBodyevent?.Execute(eventObject);
            }
        }

        public override Event[] GetChildren()
        {
            return new Event[] { loopBodyevent };
        }
    }

    [Serializable]
    public class ToggleConditionEvent : Event
    {
        [SerializeReference]
        private string tag;
        [SerializeReference]
        private bool condition;

        public override IEnumerator Execute(EventObject eventObject = null)
        {
            ConditionManager.Instance.Conditions[tag] = this.condition;
            return null;
        }
    }
}