using System.Collections;
using System.Collections.Generic;
using GyeMong.EventSystem.Interface;
using GyeMong.GameSystem.Creature.Player;
using UnityEngine;
using UnityEngine.VFX;

namespace GyeMong.GameSystem.Map
{
    public class FogController : MonoBehaviour, IEventTriggerable
    {
        [SerializeField] private VisualEffect vfxRenderer;
        private HashSet<Transform> _transformSet = new HashSet<Transform>();
        [SerializeField] private List<Transform> _transforms;
        
        private const float DELTA_TIME = 0.02f;
        private int _numOfTransform = 0;
        private int _transformIndex = 0;
        
        
        private void Start()
        {
            _transformSet.Add(PlayerCharacter.Instance.transform);
            _numOfTransform = _transforms.Count;
            _transforms = new List<Transform>(_transformSet);
        }
        
        public void AddTransform(Transform transform)
        {
            _transformSet.Add(transform);
            _transforms = new List<Transform>(_transformSet);
            _numOfTransform = _transformSet.Count;
        }
        
        public void RemoveTransform(Transform transform)
        {
            _transformSet.Remove(transform);
            _transforms.Remove(transform);
            _numOfTransform = _transformSet.Count;
        }
        
        private IEnumerator ClearFog()
        {
            float timer = 0;
            float clearTime = 5f;
            float currentRadius = vfxRenderer.GetFloat("Radius");
            float targetRadius = 10;
            float deltaRadius = (targetRadius - currentRadius) * (DELTA_TIME/clearTime);
            while (timer < clearTime)
            {
                vfxRenderer.SetFloat("Radius", currentRadius += deltaRadius);
                yield return new WaitForSeconds(DELTA_TIME);
                timer += DELTA_TIME;
            }
        }

        private void Update()
        {
            // Update Player's Position to Fog
            if (_transformIndex >= _numOfTransform)
            {
                _transformIndex = 0;
            }
            vfxRenderer.SetVector3("ColliderPos", _transforms[_transformIndex++].position-transform.position);
        }
        
        

        public void Trigger()
        {
            StartCoroutine(ClearFog());
        }
    }
}
