namespace Creature.Mob.StateMachineMob.Minion.Slime
{
    public class Slime : SlimeBase
    {
        public class  SlimeRangedAttackState : RangedAttackState { }
        public class SlimeMeleeAttackState : MeleeAttackState {}
        public class DieState : SlimeDieState { }
        public class MoveState : SlimeMoveState { }
        public class SlimeIdleState : IdleState { }
    }
}
