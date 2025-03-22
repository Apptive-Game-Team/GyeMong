using UnityEngine;

namespace System.Event.Controller.Condition
{
    [Serializable]
    public abstract class Condition
    {
        public abstract bool Check();
    }

    [Serializable]
    public class BoolCondition : Condition
    {
        [SerializeReference]
        private bool condition = true;

        public override bool Check()
        {
            return condition;
        }
    }

    [Serializable]
    public class NotCondition : Condition
    {
        [SerializeReference]
        private Condition condition;

        public override bool Check()
        {
            return !condition.Check();
        }
    }

    [Serializable]
    public class ToggeableCondition : Condition
    {
        [SerializeField]
        private string tag;
    
        [SerializeField]
        private bool condition = false;

        public override bool Check()
        {
            if (ConditionManager.Instance.Conditions.ContainsKey(tag))
            {
                condition = ConditionManager.Instance.Conditions[tag];
            }
            else 
                ConditionManager.Instance.Conditions[tag] = condition;
            return condition;
        }
        public string GetTag()
        {
            return tag;
        }
        public void SetCondition(bool condition)
        {
            this.condition = condition;
        }
    }
}