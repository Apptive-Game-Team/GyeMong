using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Condition
{
    [SerializeField]
    private bool condition = true;
    public virtual bool Check()
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
