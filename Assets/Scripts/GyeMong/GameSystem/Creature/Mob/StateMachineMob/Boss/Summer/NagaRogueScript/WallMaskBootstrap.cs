using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
[DefaultExecutionOrder(-1000)]
public class WallMaskBootstrap : MonoBehaviour
{
    public LayerMask wallLayer;      // Walls 레이어
    [Range(1,4)] public int downscale = 1; // 1=풀, 2=1/2, 4=1/4

    Camera _main;
    Camera _maskCam;
    RenderTexture _rt;

    static readonly int WallMaskTexId = Shader.PropertyToID("_WallMaskTex");

    void Awake()
    {
        _main = GetComponent<Camera>();
        CreateOrResizeRT();
        CreateMaskCamera();
        Shader.SetGlobalTexture(WallMaskTexId, _rt);
    }

    void CreateMaskCamera()
    {
        var go = new GameObject("WallMaskCamera");
        go.transform.SetParent(transform, false);
        _maskCam = go.AddComponent<Camera>();

        // 화면에는 안 그리고 RT로만 출력
        _maskCam.clearFlags = CameraClearFlags.SolidColor;
        _maskCam.backgroundColor = Color.black;
        _maskCam.cullingMask = wallLayer; // 벽만
        _maskCam.targetTexture = _rt;
        _maskCam.enabled = true;
        _maskCam.depth = -1000; // 화면 렌더 순서에 영향 없음

        // URP 설정(같은 Renderer를 쓰도록 ‘시도’)
        var mainData = _main.GetUniversalAdditionalCameraData();
        var maskData = _maskCam.GetUniversalAdditionalCameraData();
        maskData.renderPostProcessing = false;
        maskData.antialiasing = AntialiasingMode.None;

        // ⚠ 여기서 renderer를 강제로 맞추는 코드는 버전마다 API가 달라서 뺐다.
        // 대부분은 파이프라인 기본 Renderer가 같아서 문제 없음.
        // (필요하면 아래 “선택: Renderer 수동 지정” 참고)
    }

    void CreateOrResizeRT()
    {
        int w = Mathf.Max(8, Screen.width  / Mathf.Max(1, downscale));
        int h = Mathf.Max(8, Screen.height / Mathf.Max(1, downscale));

        if (_rt != null && (_rt.width != w || _rt.height != h))
        {
            _rt.Release();
            Destroy(_rt);
            _rt = null;
        }
        if (_rt == null)
        {
            _rt = new RenderTexture(w, h, 0, RenderTextureFormat.R8)
            {
                name = "WallMaskRT",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
        }
    }

    void Update()
    {
        // 해상도 변화 대응
        int w = Mathf.Max(8, Screen.width  / Mathf.Max(1, downscale));
        int h = Mathf.Max(8, Screen.height / Mathf.Max(1, downscale));
        if (_rt.width != w || _rt.height != h)
        {
            CreateOrResizeRT();
            if (_maskCam) _maskCam.targetTexture = _rt;
            Shader.SetGlobalTexture(WallMaskTexId, _rt);
        }
    }

    // CinemachineBrain이 LateUpdate에서 렌즈/행렬을 세팅한 ‘직후’ 동기화
    void OnPreCull()
    {
        if (!_maskCam) return;

        _maskCam.transform.SetPositionAndRotation(transform.position, transform.rotation);

        // 행렬까지 1:1 복사 → 완벽 싱크
        _maskCam.projectionMatrix     = _main.projectionMatrix;
        _maskCam.worldToCameraMatrix  = _main.worldToCameraMatrix;

        _maskCam.orthographic         = _main.orthographic;
        _maskCam.orthographicSize     = _main.orthographicSize;

        Shader.SetGlobalTexture(WallMaskTexId, _rt);
    }
}
