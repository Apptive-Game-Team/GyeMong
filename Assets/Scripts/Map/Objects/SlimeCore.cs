using UnityEngine;
using DG.Tweening;

namespace Map.Objects
{
    public class SlimeCore : BreakableObject
    {
        [SerializeField] private GameObject _slimeCoreItem;
        public override void DestroyEvent()
        {
            Destroy(gameObject.GetComponent<Animator>());
            GameObject slimeCore = Instantiate(_slimeCoreItem, transform.position, Quaternion.identity, transform);
            slimeCore.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
            slimeCore.transform.DOMoveY(transform.position.y + 1f, 0.3f).SetEase(Ease.OutQuad)
            .OnComplete(() => slimeCore.transform.DOMoveY(transform.position.y, 0.3f).SetEase(Ease.InBounce));
            Destroy(gameObject.GetComponent<CircleCollider2D>());
        }
    }
}

