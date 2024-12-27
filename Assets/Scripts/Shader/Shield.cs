using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public GameObject boss;
    private Material shieldMaterial;
    private bool isShieldActive = false;

    private void Awake()
    {
        shieldMaterial = boss.GetComponent<Renderer>().material;
    }

    public void OnButtonClick()
    {
        isShieldActive = !isShieldActive;
        ChangeShieldState(isShieldActive);
    }

    private void ChangeShieldState(bool flag)
    {
        Debug.Log("Shield 상태 변경: " + flag);
        shieldMaterial.SetFloat("_isShieldActive", flag ? 1.0f : 0.0f);
    }
}
