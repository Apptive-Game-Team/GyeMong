using System.Collections.Generic;
using System.Input;
using Map.Objects;
using Map.Stage.Select.Node;
using UnityEngine;

namespace Map.Stage.Select
{
    // This class is responsible for selecting the stage node based on the input.
    public class StageSelectController : MonoBehaviour
    {   
        [SerializeField] private List<StageNode> stageNodes = new List<StageNode>();
        [SerializeField] private QueuedSmoothMover cursor;
        
        [SerializeField] private float inputDelay = 0.5f;
        private float _lastInputTime;
        
        private int _maxIndex;
        
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
        }

        private void HandleKeyInput()
        {
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

            if (InputManager.Instance.GetKeyDown(ActionCode.Interaction))
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
            _maxIndex = PlayerPrefs.GetInt("MaxStageId", 0);
            _nodeSelector = new LinearNodeSelector(stageNodes, _maxIndex);
            _currentNode = stageNodes[0];
            cursor.MoveTo(_currentNode.transform.position);
            foreach (StageNode stageNode in stageNodes)
            {
                stageNode.SetStageSelectController(this);
            }
        }
    }
}