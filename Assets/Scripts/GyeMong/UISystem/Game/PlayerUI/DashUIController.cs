using System.Collections;
using System.Collections.Generic;
using GyeMong.GameSystem.Creature.Player;
using UnityEngine;

public class DashUIController : MonoBehaviour
{
    private PlayerCharacter player;
    [SerializeField] private List<GameObject> dashIcons; 

    private void Start()
    {
        player = SceneContext.Character;
        player.OnDashChanged += UpdateDashUI;
        UpdateDashUI(player.CurDash); 
    }

    private void UpdateDashUI(int curDash)
    {
        for (int i = 0; i < dashIcons.Count; i++)
        {
            dashIcons[i].SetActive(i < curDash);
        }
    }

    private void OnDestroy()
    {
        player.OnDashChanged -= UpdateDashUI; 
    }
}
