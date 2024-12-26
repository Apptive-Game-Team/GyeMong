using playerCharacter;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueButton : MonoBehaviour
{
    private PlayerData playerData;
    private void Start() 
    {
        playerData = DataManager.Instance.LoadSection<PlayerData>("PlayerData");
        if (playerData.isFirst) gameObject.SetActive(!playerData.isFirst);
    }

    public void ContinueGame()
    {
        string sceneName = playerData.sceneName;
        SceneManager.LoadScene(sceneName);
        PlayerCharacter.Instance.LoadPlayerData();
    }
}
