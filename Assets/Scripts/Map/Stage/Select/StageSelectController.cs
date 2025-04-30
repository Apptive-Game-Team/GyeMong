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
            StageNode selectedStage = _nodeSelector.SelectNode(_currentNode, GetMoveVector());
            if (selectedStage !=  null)
            {
                cursor.MoveTo(_currentNode.transform.position);
                _currentNode = selectedStage;
            }
            
            if (InputManager.Instance.GetKeyDown(ActionCode.Interaction))
            {
                _currentNode.LoadStage();
            }
        }
        
        private Vector3 GetMoveVector()
        {
            Vector3 moveVector = Vector3.zero;
            if (InputManager.Instance.GetKeyDown(ActionCode.MoveDown))
            {
                moveVector = Vector3.down;
            }
            else if (InputManager.Instance.GetKeyDown(ActionCode.MoveUp))
            {
                moveVector = Vector3.up;
            }
            else if (InputManager.Instance.GetKeyDown(ActionCode.MoveLeft))
            {
                moveVector = Vector3.left;
            }
            else if (InputManager.Instance.GetKeyDown(ActionCode.MoveRight))
            {
                moveVector = Vector3.right;
            }

            return moveVector;
        }
        
        private void Awake()
        {
            _nodeSelector = new LinearNodeSelector(stageNodes);
            _currentNode = stageNodes[0];
            foreach (StageNode stageNode in stageNodes)
            {
                stageNode.SetStageSelectController(this);
            }
        }
    }
}