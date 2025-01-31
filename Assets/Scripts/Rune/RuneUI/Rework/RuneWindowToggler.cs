using playerCharacter;
using UI.mouse_input;
using UnityEngine.UI;

public class RuneWindowToggler : SingletonObject<RuneWindowToggler>
{
    private bool isOptionOpened;
    
    public void OpenOrCloseOption()
    {
        isOptionOpened = !isOptionOpened;
        gameObject.SetActive(isOptionOpened);
        PlayerCharacter.Instance.SetPlayerMove(!isOptionOpened);
    }
    
    protected override void Awake()
    {
        base.Awake();   
        DontDestroyOnLoad(transform.parent.gameObject);
    }
    
    private void OnEnable()
    {
        gameObject.SetActive(isOptionOpened);
        MouseInputManager.Instance.SetRaycaster(GetComponent<GraphicRaycaster>());
    }
}
