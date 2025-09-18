using System;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.EventSystem.Controller.Condition
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
    public class AndConditions : Condition
    {
        [SerializeField] 
        private List<string> tags;

        public override bool Check()
        {
            foreach (var tag in tags)
            {
                if (!ConditionManager.Instance.Conditions.ContainsKey(tag)) return false;
                if (!ConditionManager.Instance.Conditions[tag]) return false;
            }
            return true;
        }
    }

    [Serializable]
    public class OrConditions : Condition
    {
        [SerializeField] 
        private List<string> tags;

        public override bool Check()
        {
            foreach (var tag in tags)
            {
                if (!ConditionManager.Instance.Conditions.ContainsKey(tag)) return false;
                if (!ConditionManager.Instance.Conditions[tag]) return true;
            }
            return false;
        }
    }
}