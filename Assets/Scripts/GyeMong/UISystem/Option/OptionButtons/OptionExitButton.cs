using GyeMong.GameSystem.Creature.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GyeMong.UISystem.Option.OptionButtons
{
    public class OptionExitButton : MonoBehaviour
    {
        private void Start()
        {
            if (SceneManager.GetActiveScene().name == "TitleScene")
            {
                gameObject.SetActive(false);
            }
        }

        public void ExitButton()
        {
            gameObject.SetActive(false);
            OptionUIToggler.Instance.ToggleOption();
            Destroy(SceneContext.Character.gameObject);
            SceneManager.LoadScene("TitleScene");
        }
    }
}
