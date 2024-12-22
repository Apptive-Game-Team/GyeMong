using UnityEngine;
using System;

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

    private bool isSetUp = false;
    
    [SerializeField]
    private bool condition = false;

    public override bool Check()
    {
        if (!isSetUp)
        {
            if (ConditionManager.Instance.Conditions.ContainsKey(tag))
            {
                condition = ConditionManager.Instance.Conditions[tag];
            }
            else 
                ConditionManager.Instance.Conditions[tag] = condition;
            isSetUp = true;
        }
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