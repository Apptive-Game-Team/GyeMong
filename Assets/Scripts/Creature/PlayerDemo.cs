using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDemo : SingletonObject<PlayerDemo>
{
    private float speed = 20f;
    private float maxHealth;
    [SerializeField] private float curHealth;
    private Rigidbody2D playerRigidbody;
    private bool canMove = true;

    void Start()
    {
        maxHealth = 100f;
        curHealth = maxHealth;
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!canMove) return;

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(inputX, inputY).normalized * speed * Time.deltaTime;

        playerRigidbody.MovePosition(playerRigidbody.position + movement);
    }
    public void TakeDamage(float damage)
    {
        curHealth -= damage;
        if (curHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Destroy(gameObject);
    }
    public void Bind(float duration)
    {
        StartCoroutine(BindCoroutine(duration));
    }

    private IEnumerator BindCoroutine(float duration)
    {
        canMove = false; // 움직임 제한
        yield return new WaitForSeconds(duration); // 지정된 시간 대기
        canMove = true; // 움직임 재개
    }
}
