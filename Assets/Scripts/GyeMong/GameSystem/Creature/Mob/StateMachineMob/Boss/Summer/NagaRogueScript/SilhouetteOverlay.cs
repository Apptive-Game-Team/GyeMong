using UnityEngine;

[DisallowMultipleComponent]
public class SilhouetteOverlay : MonoBehaviour
{
    [SerializeField] public SpriteRenderer source;           // 원본
    [SerializeField] public Material silhouetteMat;          // SG_SilhouetteMasked로 만든 머티리얼
    [SerializeField] public string overlaySortingLayer = "Overlay";
    [SerializeField] public int overlayOrderBoost = 1000;    // 원본보다 위

    SpriteRenderer _sr;

    void Awake()
    {
        if (!source) source = GetComponentInParent<SpriteRenderer>();
        var g = new GameObject("SilhouetteOverlay");
        g.transform.SetParent(source.transform, false);
        _sr = g.AddComponent<SpriteRenderer>();
        _sr.sharedMaterial = silhouetteMat;
        _sr.sortingLayerName = overlaySortingLayer;
        _sr.sortingOrder = source.sortingOrder + overlayOrderBoost;
        _sr.color = Color.white; // 색은 머티리얼에서 처리
        if (WallMaskBootstrap.CurrentMaskRT)
            _sr.material.SetTexture("_WallMaskTex", WallMaskBootstrap.CurrentMaskRT);
    }

    void LateUpdate()
    {
        if (!source || !_sr) return;

        _sr.sprite = source.sprite;
        _sr.flipX = source.flipX;
        _sr.flipY = source.flipY;
        _sr.transform.localPosition = Vector3.zero;
        _sr.transform.localRotation = Quaternion.identity;
        _sr.transform.localScale = Vector3.one;

        // 필요하면 원본의 Enable/Disable 상태 맞추기
        _sr.enabled = source.enabled;
        var rt = WallMaskBootstrap.CurrentMaskRT;
        if (rt && _sr.material.GetTexture("_WallMaskTex") != rt)
            _sr.material.SetTexture("_WallMaskTex", rt);
    }
}