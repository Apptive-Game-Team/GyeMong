using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Direction
{
    public class DirectionController : MonoBehaviour
    {
        
        private float _angle = 0f; // in radians
        private float _angularVelocity = Mathf.PI; // radians per second
        
        public float Angle
        {
            get { return _angle; }
            private set
            {
                _angle = value;
            }
        }

        public Vector3 GetDirection()
        {
            return new Vector3(Mathf.Cos(_angle), Mathf.Sin(_angle), 0); 
        }
        
        public void SetAngularVelocity(float angularVelocity)
        {
            _angularVelocity = angularVelocity;
        }
        
        public IEnumerator TrackPlayer(float angularVelocity, bool continuously = false)
        {
            return TrackTarget(SceneContext.Character.transform, angularVelocity, continuously);
        }
    
        public IEnumerator TrackTarget(Transform target, float angularVelocity, bool continuously = false)
        {
            _angularVelocity = angularVelocity;
            while (true)
            {
                float targetAngle = GetTargetAngle(target);
            
                float angleDiff = Mathf.DeltaAngle(_angle * Mathf.Rad2Deg, targetAngle * Mathf.Rad2Deg) * Mathf.Deg2Rad;
                if (Mathf.Abs(angleDiff) < 0.01f)
                {
                    _angle = targetAngle;
                    if (continuously)
                    {
                        yield return null;
                        continue;
                    }
                    yield break;
                }

                float angleChange = _angularVelocity * Time.deltaTime;
                if (Mathf.Abs(angleDiff) < angleChange)
                {
                    _angle = targetAngle;
                    if (continuously)
                    {
                        yield return null;
                        continue;
                    }
                    yield break;
                }
            
                if (angleDiff > 0)
                {
                    _angle += angleChange;
                }
                else
                {
                    _angle -= angleChange;
                }

                yield return null;
            }
        }
    
        private float GetTargetAngle(Transform target)
        {
            Vector3 dir = target.position - transform.position;
            return Mathf.Atan2(dir.y, dir.x);
        }
    }
}
