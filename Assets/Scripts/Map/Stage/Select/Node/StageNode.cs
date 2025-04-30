using UnityEngine;

namespace Map.Stage.Select.Node
{
    // for Stage Node, which is a button that represents a stage
    public class StageNode : MonoBehaviour, ISelectableUI
    {
        private StageSelectController _stageSelectController;
        
        public void SetStageSelectController(StageSelectController stageSelectController)
        {
            _stageSelectController = stageSelectController;
        }

        public void OnInteract()
        {
            print("Interacted with: " + gameObject.name);
            _stageSelectController.OnStageClicked(this);
        }

        public void OnLongInteract() { }
    }
}
