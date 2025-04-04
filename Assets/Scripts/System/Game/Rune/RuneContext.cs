using System.Collections.Generic;
using System.Game.Buff;
using System.Game.Buff.Data;
using Creature.Player.Component;
using UnityEngine;

namespace System.Game.Rune
{
    //Goal. I Want to Abstract these Parameters into a single General Parameter...
    //Otherwise, I would have to use if-else...
    public abstract class RuneContext
    {
        
    }
    
    [Serializable]
    public class ModifyStatContext : RuneContext
    {
        public StatComponent StatComp { get;}
        public StatType StatType { get; } 
        public StatValueType ValueType { get; } 
        public float Value { get; }
        
        public List<StatSet> StatSetList { get; }

        //Too Many Parameters. Struct. Need.
        public ModifyStatContext(StatComponent statComp, StatType statType, StatValueType statValueType , float value)
        {
            StatComp = statComp;
            StatType = statType;
            ValueType = statValueType;
            Value = value;
        }

        public ModifyStatContext(StatComponent statComp, List<StatSet> statSetList)
        {
            StatSetList = statSetList;
        }
    }
    
    public class AddBuffContext : RuneContext
    {
        public BuffComponent BuffComp { get;}
        public BuffData BuffData { get;}

        public AddBuffContext(BuffComponent buffComp, BuffData buffData)
        {
            BuffComp = buffComp;
            BuffData = buffData;
        }
    }    
    
    //How to Make PlayerInput class?
    //Members : KeyAction, PLayerAction,
    //I Want to Change PlayerAction or KeyAction to RuneActivated-Things.
    public class ChangePlayerInputContext : RuneContext
    {
        
    }


}
