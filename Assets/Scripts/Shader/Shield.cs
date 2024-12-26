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
        // SpriteRenderer의 Material 가져오기
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
        
        // 셰이더 변수 업데이트
        shieldMaterial.SetFloat("_isShieldActive", flag ? 1.0f : 0.0f);

        // 변경 사항 강제 적용
        boss.GetComponent<Renderer>().material = shieldMaterial;
    }
}
