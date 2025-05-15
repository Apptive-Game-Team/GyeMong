using GyeMong.InputSystem.Interface;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GyeMong.GameSystem.Map.Stage.Select.Node
{
    // for Stage Node, which is a button that represents a stage
    public class StageNode : MonoBehaviour, ISelectableUI
    {
        private StageSelectController _stageSelectController;
        [SerializeField] private string sceneName;

        public void LoadStage()
        {
            SceneManager.LoadScene(sceneName);
        }
        
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
