using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
[DefaultExecutionOrder(-1000)]
public class WallMaskBootstrap : MonoBehaviour
{
    [Header("Mask contents (GameObject Layer)")]
    public LayerMask wallLayer;
    [Range(1,4)] public int downscale = 1;

    [Header("URP Renderer index from URP Asset > Renderer List")]
    [SerializeField] int rendererIndex = 0; // 네가 말한 0 유지

    Camera _main, _maskCam;
    RenderTexture _rt;

    public static RenderTexture CurrentMaskRT { get; private set; }
    static readonly int WallMaskTexId = Shader.PropertyToID("_WallMaskTex");

    void Awake()
    {
        _main = GetComponent<Camera>();
        CreateOrResizeRT();
        CreateMaskCamera();
        SyncAll("Awake");     // ★ 초기 프레임부터 맞춤
        PushGlobal();
    }

    void OnEnable()  { PushGlobal(); }
    void OnDisable() { Shader.SetGlobalTexture(WallMaskTexId, null); }

    void OnDestroy()
    {
        if (_maskCam) Destroy(_maskCam.gameObject);
        if (_rt) { _rt.Release(); Destroy(_rt); }
    }

    void CreateMaskCamera()
    {
        var go = new GameObject("WallMaskCamera");
        go.hideFlags = HideFlags.DontSave;
        go.transform.SetParent(transform, false);

        _maskCam = go.AddComponent<Camera>();
        _maskCam.enabled         = true;
        _maskCam.clearFlags      = CameraClearFlags.SolidColor;
        _maskCam.backgroundColor = Color.black;
        _maskCam.cullingMask     = wallLayer;
        _maskCam.targetTexture   = _rt;
        _maskCam.depth           = -1000;

        var maskData = _maskCam.GetUniversalAdditionalCameraData();
        maskData.renderPostProcessing = false;
        maskData.antialiasing         = AntialiasingMode.None;
        maskData.SetRenderer(rendererIndex);   // ★ 강제 동일 Renderer 사용
    }

    void CreateOrResizeRT()
    {
        int w = Mathf.Max(8, Screen.width  / Mathf.Max(1, downscale));
        int h = Mathf.Max(8, Screen.height / Mathf.Max(1, downscale));

        if (_rt != null && (_rt.width != w || _rt.height != h))
        { _rt.Release(); Destroy(_rt); _rt = null; }

        if (_rt == null)
        {
            _rt = new RenderTexture(w, h, 0, RenderTextureFormat.R8)
            { name = "WallMaskRT", filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp };
        }
        CurrentMaskRT = _rt;
    }

    void LateUpdate()
    {
        // Cinemachine이 LateUpdate에서 렌즈를 건드리므로 여기서도 동기화
        SyncAll("LateUpdate");
    }

    void OnPreCull()
    {
        // 렌더 직전에 한 번 더 보증
        SyncAll("OnPreCull");
        PushGlobal();
    }

    void SyncAll(string tag)
    {
        if (!_maskCam || !_main) return;

        // 위치/회전/뷰-프로젝션/오쏘 옵션/클리핑/뷰포트까지 전부 맞춤
        _maskCam.transform.SetPositionAndRotation(transform.position, transform.rotation);

        _maskCam.orthographic        = _main.orthographic;
        _maskCam.orthographicSize    = _main.orthographicSize;
        _maskCam.nearClipPlane       = _main.nearClipPlane;
        _maskCam.farClipPlane        = _main.farClipPlane;
        _maskCam.rect                = _main.rect;

        _maskCam.projectionMatrix    = _main.projectionMatrix;
        _maskCam.worldToCameraMatrix = _main.worldToCameraMatrix;

        // (선택) Pixel Perfect Camera를 메인에 쓰면, 같은 값으로 하나 더 붙여도 됨
        // var ppMain = _main.GetComponent<UnityEngine.U2D.PixelPerfectCamera>();
        // if (ppMain && !_maskCam.GetComponent<UnityEngine.U2D.PixelPerfectCamera>())
        //     UnityEngine.U2D.PixelPerfectCameraUtilities.AddCopy(ppMain, _maskCam.gameObject);

        // Base 카메라인지 디버그
        var mainData = _main.GetUniversalAdditionalCameraData();
        if (mainData.renderType != CameraRenderType.Base)
        {
            Debug.LogWarning($"[WallMask] Script is on an Overlay camera. Put it on the BASE camera. ({tag})");
        }
    }

    void PushGlobal()
    {
        Shader.SetGlobalTexture(WallMaskTexId, _rt);
        CurrentMaskRT = _rt;
    }
}
