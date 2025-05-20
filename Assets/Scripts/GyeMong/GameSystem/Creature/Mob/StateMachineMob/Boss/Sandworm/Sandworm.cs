using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Sandworm
{
    public class Sandworm : Boss
    {
        protected override void Initialize()
        {
            maxPhase = 2;
            maxHps.Clear();
            maxHps.Add(100f);
            maxHps.Add(200f);
            currentHp = maxHps[currentPhase];
            damage = 20f;
            speed = 2f;
            currentShield = 0f;
            detectionRange = 10f;
            MeleeAttackRange = 2f;
            RangedAttackRange = 8f;

            ChangeState(new VenomBreath(){mob = this});
        }

        public abstract class SandwormState : CoolDownState
        {
            public Sandworm Sandworm => mob as Sandworm;
            protected Dictionary<System.Type, int> weights;

            public override void OnStateUpdate()
            {
                if (Sandworm.DirectionToPlayer.x > 0) Sandworm.GetComponent<SpriteRenderer>().flipX = true;
                else Sandworm.GetComponent<SpriteRenderer>().flipX = false;
                print("tqq");
            }

            protected virtual void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                    {
                        {typeof(VenomBreath), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 5 : 0 },
                        {typeof(HeadAttack), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 5 : 0 },
                        {typeof(FlameLaser), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 5 : 0 },
                        {typeof(ShortBurstOutAttack), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 5 : 0 },
                        {typeof(LongBurstOutAttack), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 5 : 0 },
                        {typeof(SandTrapAttack), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 5 : 0 }
                    };
            }
            
            protected Dictionary<System.Type, int> NextStateWeights
            {
                get
                {   
                    return weights;
                }
            }
        }

        public class VenomBreath : SandwormState
        {
            public override int GetWeight()
            {
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Sandworm.ChangeState(NextStateWeights);
                yield return null;
            }
        }

        public class HeadAttack : SandwormState
        {
            public override int GetWeight()
            {
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Sandworm.ChangeState(NextStateWeights);
                yield return null;
            }
        }

        public class FlameLaser : SandwormState
        {
            public override int GetWeight()
            {
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Sandworm.ChangeState(NextStateWeights);
                yield return null;
            }
        }

        public class ShortBurstOutAttack : SandwormState
        {
            public override int GetWeight()
            {
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Sandworm.ChangeState(NextStateWeights);
                yield return null;
            }
        }

        public class LongBurstOutAttack : SandwormState
        {
            public override int GetWeight()
            {
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Sandworm.ChangeState(NextStateWeights);
                yield return null;
            }
        }

        public class SandTrapAttack : SandwormState
        {
            public override int GetWeight()
            {
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Sandworm.ChangeState(NextStateWeights);
                yield return null;
            }
        }
    }
}
