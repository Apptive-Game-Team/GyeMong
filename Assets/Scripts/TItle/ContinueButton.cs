using playerCharacter;
using System.Threading.Tasks;
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

    public async void ContinueGame()
    {
        string sceneName = playerData.sceneName;

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is invalid.");
            return;
        }

        // 비동기적으로 장면 로드
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        // 장면이 완전히 로드될 때까지 대기
        while (!asyncOperation.isDone)
        {
            await Task.Yield();
        }

        // 장면 로드가 완료된 뒤 플레이어 데이터 로드
        DataManager.Instance.LoadPlayerData();
    }
}
