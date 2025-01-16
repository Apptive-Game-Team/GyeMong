using System.Collections;
using UnityEngine;

public class SlimeAnimator : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private SlimeSprites sprites;
    private Coroutine currentAnimation;
    private bool stopCurrentAnimation = false;
    
    public static SlimeAnimator Create(GameObject parent, SlimeSprites sprites)
    {
        SlimeAnimator animator = parent.gameObject.AddComponent<SlimeAnimator>();
        animator.spriteRenderer = parent.GetComponent<SpriteRenderer>();
        animator.sprites = sprites;
        return animator;
    }

    public enum AnimationType
    {
        IDLE,
        MELEE_ATTACK,
        RANGED_ATTACK,
        DIE
    }
    
    public IEnumerator SyncPlay(AnimationType type, bool loop = false)
    {
        do
        {
            yield return PlayerAnimation(sprites.GetSprite(type));
        } while (loop);
    }
    

    
    public void AsyncPlay(AnimationType type, bool loop = false)
    {
        if (currentAnimation != null)
        {
            Stop();
        }
        currentAnimation = StartCoroutine(SyncPlay(type, loop));
    }
    
    public void Stop()
    {
        StopCoroutine(currentAnimation);
        currentAnimation = null;
    }

    private IEnumerator PlayerAnimation(Sprite[] sprites)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            spriteRenderer.sprite = sprites[i];
            yield return new WaitForSeconds(0.1f);
        }
    }
}
