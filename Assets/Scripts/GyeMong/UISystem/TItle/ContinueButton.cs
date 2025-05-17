using GyeMong.GameSystem.Map.Stage.Select;
using UnityEngine;

namespace GyeMong.UISystem.TItle
{
    public class ContinueButton : MonoBehaviour
    {
        private GameObject optionExitButton;
        private void Start() 
        {
            int maxStageId = PlayerPrefs.GetInt(StageSelectPage.MAX_STAGE_ID_KEY, 0);
            gameObject.SetActive(maxStageId > 0);
            optionExitButton = GameObject.Find("OptionController").transform.GetChild(0).GetChild(0).Find("OptionExitButton").gameObject;
        }

        public void ContinueGame()
        {
            optionExitButton.SetActive(true);

            StageSelectPage.LoadStageSelectPageOnStage(
                PlayerPrefs.GetInt(StageSelectPage.MAX_STAGE_ID_KEY, 0)
                );
        }
    }
}
