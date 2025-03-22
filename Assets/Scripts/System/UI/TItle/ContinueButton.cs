using playerCharacter;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueButton : MonoBehaviour
{
    private PlayerData playerData;
    private GameObject optionExitButton;
    private void Start() 
    {
        playerData = DataManager.Instance.LoadSection<PlayerData>("PlayerData");
        if (playerData.isFirst) gameObject.SetActive(!playerData.isFirst);
        optionExitButton = GameObject.Find("OptionController").transform.GetChild(0).GetChild(0).Find("OptionExitButton").gameObject;
    }

    public async void ContinueGame()
    {
        string sceneName = playerData.sceneName;

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is invalid.");
            return;
        }

        // �񵿱������� ��� �ε�
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        // ����� ������ �ε�� ������ ���
        while (!asyncOperation.isDone)
        {
            await Task.Yield();
        }

        optionExitButton.SetActive(true);
        // ��� �ε尡 �Ϸ�� �� �÷��̾� ������ �ε�
        DataManager.Instance.LoadPlayerData();
    }
}
