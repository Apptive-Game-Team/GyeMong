using System.Collections;
using System.Game.Object.Persisted;
using System.Game.Quest.Component;
using playerCharacter;
using UnityEngine.SceneManagement;
public class SaveDataEvent : Event
{
    private PlayerData playerData = new();
    private RuneDatas runeData = new();
    private QuestData questData = new();
    private RuneComponent runeComponent;
    private QuestComponent questComponent;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        playerData.isFirst = false;
        playerData.sceneName = SceneManager.GetActiveScene().name;
        playerData.playerPosition = PlayerCharacter.Instance.GetPlayerPosition();
        playerData.playerDirection = PlayerCharacter.Instance.GetPlayerDirection();

        runeComponent = PlayerCharacter.Instance.GetComponent<RuneComponent>();
        questComponent = PlayerCharacter.Instance.GetComponent<QuestComponent>();
        
        runeData.AcquiredRuneDatas = runeComponent.AcquiredRuneList;
        runeData.EquippedRuneDatas = runeComponent.EquippedRuneList;

        questData.quests = questComponent.GetQuestInfos();
        
        DataManager.Instance.SaveSection(questData, "QuestData");
        DataManager.Instance.SaveSection(playerData, "PlayerData");
        DataManager.Instance.SaveSection(runeData, "RuneData");
        
        ConditionManager.Instance.Save();
        
        PersistedGameObjectManager.Instance.Save();

        yield return null;
    }
}
public class LoadDataEvent : Event
{
    private PlayerData playerData;
    
    private RuneDatas runeData;
    private RuneComponent runeComponent;
    
    private QuestData questData;
    private QuestComponent questComponent;
    public override IEnumerator Execute(EventObject eventObject = null)
    {
        playerData = DataManager.Instance.LoadSection<PlayerData>("PlayerData");
        runeData = DataManager.Instance.LoadSection<RuneDatas>("RuneData");
        questData = DataManager.Instance.LoadSection<QuestData>("QuestData");
        
        runeComponent = PlayerCharacter.Instance.GetComponent<RuneComponent>();
        questComponent = PlayerCharacter.Instance.GetComponent<QuestComponent>();
        
        for (int i = 0;i < runeData.AcquiredRuneDatas.Count;i++) 
        {
            runeComponent.AcquireRune(runeData.AcquiredRuneDatas[i]);
        }
        for (int i = 0;i < runeData.EquippedRuneDatas.Count;i++)
        {
            runeComponent.EquipRune(runeData.EquippedRuneDatas[i]);
        }
        
        questComponent.SetQuests(questData.quests);

        SceneManager.LoadScene(playerData.sceneName);
        DataManager.Instance.LoadPlayerData();
        
        PersistedGameObjectManager.Instance.SetPersistedGameObjects(DataManager.Instance.LoadSection<PersistedGameObjectDatas>(PersistedGameObjectManager.PERSISTED_DATA_FILE));
        yield return null;
    }
}


