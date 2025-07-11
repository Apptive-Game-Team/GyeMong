using System;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace GyeMong.GameSystem.Indicator
{
    public class BoxCollider : IIndicatorShape
    {
        private GameObject _boxObject;
        public BoxCollider(GameObject boxObject) => _boxObject = boxObject;
        public GameObject CreateIndicator(GameObject attackObject, Vector3 pos, Quaternion rot)
        {
            var box = attackObject.GetComponent<BoxCollider2D>();
            if (box == null) return null;

            var indicator = UnityEngine.Object.Instantiate(_boxObject);
            indicator.transform.position = pos + (Vector3)box.offset;
            indicator.transform.rotation = rot;
            indicator.transform.localScale = Vector3.Scale(new Vector3(box.size.x, box.size.y, 1f), attackObject.transform.lossyScale);

            return indicator;
        }
    }

    public class CircleCollider : IIndicatorShape
    {
        private GameObject _circleObject;
        public CircleCollider(GameObject circleObject) => _circleObject = circleObject;
        public GameObject CreateIndicator(GameObject attackObject, Vector3 pos, Quaternion rot)
        {
            var circle = attackObject.GetComponent<CircleCollider2D>();
            if (circle == null) return null;

            var indicator = UnityEngine.Object.Instantiate(_circleObject);
            indicator.transform.position = pos + (Vector3)circle.offset;
            indicator.transform.rotation = rot;

            float diameter = circle.radius * 2f;
            float scale = Mathf.Max(attackObject.transform.lossyScale.x, attackObject.transform.lossyScale.y);
            indicator.transform.localScale = new Vector3(diameter * scale, diameter * scale, 1f);

            return indicator; 
        }
    }

    public class CapsuleCollider : IIndicatorShape
    {
        private GameObject _capsuleObject;
        public CapsuleCollider(GameObject capsuleObject) => _capsuleObject = capsuleObject;
        public GameObject CreateIndicator(GameObject attackObject, Vector3 pos, Quaternion rot)
        {
            var capsule = attackObject.GetComponent<CapsuleCollider2D>();
            if (capsule == null) return null;

            var indicator = UnityEngine.Object.Instantiate(_capsuleObject);
            indicator.transform.position = pos + (Vector3)capsule.offset;
            indicator.transform.rotation = rot * Quaternion.Euler(0f, 0f, 90f);

            SpriteRenderer sr = indicator.GetComponent<SpriteRenderer>();
            Vector2 spriteSize = sr.sprite.bounds.size;
            
            Vector3 desiredSize = capsule.direction == CapsuleDirection2D.Horizontal ? 
                new Vector3(capsule.size.y, capsule.size.x, 1f) : new Vector3(capsule.size.x, capsule.size.y, 1f);

            Vector3 scaledSize = Vector3.Scale(desiredSize, attackObject.transform.lossyScale);
            
            Vector3 scale = new Vector3(
                scaledSize.x / spriteSize.x,
                scaledSize.y / spriteSize.y,
                1f
            );

            indicator.transform.localScale = scale;

            return indicator;
        }
    }
    
    public class IndicatorGenerator : SingletonObject<IndicatorGenerator>
    {
        [SerializeField] private GameObject boxPrefab;
        [SerializeField] private GameObject circlePrefab;
        [SerializeField] private GameObject capsulePrefab;
        
        private Dictionary<Type, IIndicatorShape> _shapeMap = new();

        protected override void Awake()
        {
            _shapeMap[typeof(BoxCollider2D)] = new BoxCollider(boxPrefab);
            _shapeMap[typeof(CircleCollider2D)] = new CircleCollider(circlePrefab);
            _shapeMap[typeof(CapsuleCollider2D)] = new CapsuleCollider(capsulePrefab);
        }
        

        public void GenerateIndicator(GameObject attackObject, Vector3 pos, Quaternion rot, float duration)
        {
            Collider2D col = attackObject.GetComponent<Collider2D>();
            if (col == null)
            {
                Debug.LogWarning("Collider2D not found");
                return;
            }

            var type = col.GetType();
            if (_shapeMap.TryGetValue(type, out var shape))
            {
                GameObject indicator = shape.CreateIndicator(attackObject, pos, rot);
                StartCoroutine(indicator.AddComponent<Indicator>().Flick(duration));
            }
            else
            {
                Debug.LogWarning($"No provider registered for {type}");
            }
        }
    }
}
