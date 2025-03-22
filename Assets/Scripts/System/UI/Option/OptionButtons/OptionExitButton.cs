using Creature.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace System.UI.Option.OptionButtons
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
            Destroy(PlayerCharacter.Instance.gameObject);
            SceneManager.LoadScene("TitleScene");
        }
    }
}
