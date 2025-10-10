using UnityEngine;

public class ReturnDagger : MonoBehaviour
{
    [SerializeField] private LineRenderer ropeLine;
    private Transform _bossTr;

    public void Initiate(Transform bossTr)
    {
        _bossTr = bossTr;
    }
    private static readonly int BASE_COLOR  = Shader.PropertyToID("_BaseColor");  // URP Unlit/Lit 등
    private static readonly int COLOR       = Shader.PropertyToID("_Color");      // Built-in/Particles
    private static readonly int TINT_COLOR  = Shader.PropertyToID("_TintColor");  // 일부 파티클 셰이더
    private MaterialPropertyBlock _mpb;

    private void SetLineColor(Color c)
    {
        if (_mpb == null) _mpb = new MaterialPropertyBlock();

        // 머티리얼 컬러 속성 주입
        ropeLine.GetPropertyBlock(_mpb);
        var mat = ropeLine.sharedMaterial; // 속성 존재 여부 확인용

        if (mat != null && mat.HasProperty(BASE_COLOR)) _mpb.SetColor(BASE_COLOR, c);
        if (mat != null && mat.HasProperty(COLOR))      _mpb.SetColor(COLOR, c);
        if (mat != null && mat.HasProperty(TINT_COLOR)) _mpb.SetColor(TINT_COLOR, c);

        ropeLine.SetPropertyBlock(_mpb);

        // (보정) 라인렌더러 버텍스컬러 경로도 활성화
        var g = new Gradient();
        g.SetKeys(new [] { new GradientColorKey(c, 0f), new GradientColorKey(c, 1f) },
            new [] { new GradientAlphaKey(c.a, 0f), new GradientAlphaKey(c.a, 1f) });
        ropeLine.colorGradient = g;
    }
    void Update()
    {
        if (ropeLine.enabled)
        {
            ropeLine.SetPosition(0, _bossTr.position);
            ropeLine.SetPosition(1, transform.position);
            var pl = SceneContext.Character.gameObject;
            if (pl != null)
            {
                Vector3 a = _bossTr.position;
                Vector3 b = transform.position;
                Vector3 p = pl.transform.position;

                Vector3 ab = b - a;
                float abSqr = ab.sqrMagnitude;
                if (abSqr > 0)
                {
                    float t = Mathf.Clamp01(Vector3.Dot(p - a, ab) / abSqr);
                    Vector3 closest = a + t * ab;
                    float dist = Vector3.Distance(p, closest);

                    float threshold = Mathf.Max(ropeLine.widthMultiplier * 0.5f, 0.25f);
                    if (dist <= threshold)
                    {
                        SetLineColor(Color.red);   // ← 기존: ropeLine.material = ropeLine.materials[1];
                    }
                    else
                    {
                        SetLineColor(Color.white); // ← 기존: ropeLine.material = ropeLine.materials[0];
                    }

                }
            }
        }
    }
}
