using playerCharacter;

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
    }
}
