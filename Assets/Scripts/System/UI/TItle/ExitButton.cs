using UnityEngine;

namespace System.UI.TItle
{
    public class ExitButton : MonoBehaviour
    {
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}
