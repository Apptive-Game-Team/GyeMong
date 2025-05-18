using System.Collections.Generic;
using GyeMong.GameSystem.Map.Stage.Select.Node;
using GyeMong.InputSystem;
using Map.Objects;
using UnityEngine;

namespace GyeMong.GameSystem.Map.Stage.Select
{
    // This class is responsible for selecting the stage node based on the input.
    public class StageSelectController : MonoBehaviour
    {   
        [SerializeField] private List<StageNode> stageNodes = new List<StageNode>();
        [SerializeField] private QueuedSmoothMover cursor;
        
        [SerializeField] private float inputDelay = 0.5f;
        
        private float _lastInputTime;
        
        [SerializeField] private int _maxIndex;
        
        private StageNode _currentNode;
        private INodeSelector<StageNode> _nodeSelector;

        public void OnStageClicked(StageNode clickedNode)
        {
            _currentNode = clickedNode;
            cursor.MoveTo(_currentNode.transform.position);
        }

        private void Update()
        {

            HandleKeyInput();
#if UNITY_EDITOR
            (_nodeSelector as LinearNodeSelector).SetMaxIndex(_maxIndex);

#endif
        }

        private void HandleKeyInput()
        {
#if UNITY_EDITOR
            if (Time.time - _lastInputTime < inputDelay) return;

            Vector3 moveVector = GetMoveVector();
            if (moveVector != Vector3.zero)
            {
                StageNode selectedStage = _nodeSelector.SelectNode(_currentNode, moveVector);
                if (selectedStage != null)
                {
                    _lastInputTime = Time.time;
                    Debug.Log("Selected stage: " + selectedStage.gameObject.name);
                    _currentNode = selectedStage;
                    cursor.MoveTo(_currentNode.transform.position);
                }
            }
#endif
            if (InputManager.Instance.GetKeyDown(ActionCode.Enter))
            {
                _currentNode.LoadStage();
            }
        }
        
        private Vector3 GetMoveVector()
        {
            Vector3 moveVector = Vector3.zero;
            if (InputManager.Instance.GetKey(ActionCode.MoveDown))
            {
                moveVector += Vector3.down;
            }
            if (InputManager.Instance.GetKey(ActionCode.MoveUp))
            {
                moveVector += Vector3.up;
            } 
            if (InputManager.Instance.GetKey(ActionCode.MoveLeft))
            {
                moveVector += Vector3.left;
            }
            if (InputManager.Instance.GetKey(ActionCode.MoveRight))
            {
                moveVector += Vector3.right;
            }

            return moveVector.normalized;
        }
        
        private void Awake()
        {
            cursor.SetPosition(stageNodes[PlayerPrefs.GetInt("CurrentStageId", 0)].transform.position);
            _maxIndex = PlayerPrefs.GetInt(StageSelectPage.MAX_STAGE_ID_KEY, 1);
            _nodeSelector = new LinearNodeSelector(stageNodes, _maxIndex);
            _currentNode = stageNodes[_maxIndex];
            cursor.MoveTo(_currentNode.transform.position);
            foreach (StageNode stageNode in stageNodes)
            {
                stageNode.SetStageSelectController(this);
            }
        }
    }
}