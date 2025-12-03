using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GyeMong.GameSystem.Creature.Player.Component
{
    public class OcclusionWall : MonoBehaviour
    {
        private static int _posID = Shader.PropertyToID("_PlayerPos");
        private static int _sizeID = Shader.PropertyToID("_Size");
        
        public Material wallMaterial;
        public LayerMask mask;
        
        private Camera _camera;
        private SpriteRenderer _playerRenderer;
        private List<SpriteRenderer> _wallRenderers = new();

        private void Awake()
        {
            _camera = Camera.main;
            _playerRenderer = SceneContext.Character.transform.GetComponent<SpriteRenderer>();
            
            SpriteRenderer[] all = GameObject.FindObjectsOfType<SpriteRenderer>();
            foreach (var sr in all)
            {
                if (((1 << sr.gameObject.layer) & mask) != 0)
                {
                    _wallRenderers.Add(sr);
                }
            }
        }

        private void Update()
        {
            bool isBlocked = false;

            foreach (var wall in _wallRenderers)
            {
                // 화면에서 bounds가 겹치는지
                if (!wall.bounds.Intersects(_playerRenderer.bounds))
                {
                    continue;
                }

                // order에서 벽이 더 앞에 있는지
                if (wall.sortingOrder < _playerRenderer.sortingOrder)
                {
                    continue;
                } 
                
                if (wall.sortingOrder == _playerRenderer.sortingOrder)
                {
                    // order가 같다면 벽의 y좌표가 더 밑에 있는지
                    if (wall.transform.position.y >= _playerRenderer.transform.position.y)
                    {
                        continue;
                    }
                }
                
                // → 이 벽은 플레이어를 가리고 있음
                isBlocked = true;
                break;
            }
            
            wallMaterial.SetFloat(_sizeID, isBlocked ? 0.5f : 0);
            
            var view = _camera.WorldToViewportPoint(transform.position);
            wallMaterial.SetVector(_posID, view);
        }
    }
}
