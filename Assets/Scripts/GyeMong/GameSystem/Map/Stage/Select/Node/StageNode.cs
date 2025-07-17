using GyeMong.GameSystem.Map.Stage.ScriptableObject;
using GyeMong.InputSystem.Interface;
using UnityEngine;

namespace GyeMong.GameSystem.Map.Stage.Select.Node
{
    // for Stage Node, which is a button that represents a stage
    public class StageNode : MonoBehaviour, ISelectableUI
    {
        private StageSelectController _stageSelectController;
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;
        
        [SerializeField] private StageInfo stageInfo;
        public void LoadStage()
        {
            StageManager.EnterStage(stageInfo, this);
        }
        
        public void SetStageSelectController(StageSelectController stageSelectController)
        {
            _stageSelectController = stageSelectController;
        }
        
        public void SetOnOff(bool isOn)
        {
            GetComponent<SpriteRenderer>().sprite = isOn ? onSprite : offSprite;
        }

        public void OnInteract()
        {
            print("Interacted with: " + gameObject.name);
            _stageSelectController.OnStageClicked(this);
        }

        public void OnLongInteract() { }
    }
}
