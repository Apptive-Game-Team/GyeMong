Shader "Custom/RoundedIndicator"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _HalfSize ("Half Size (world units)", Vector) = (0.5, 0.5, 0, 0)
        _CornerRadius ("Corner Radius (world units)", Float) = 0.25
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane"}
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 local : TEXCOORD1;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float4 _HalfSize;
            float _CornerRadius;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;

                // 인디케이터는 콜라이더 크기에 맞춰 비균등 스케일로 늘어난다.
                // 모서리 반경을 월드 단위로 유지하려고, 회전과 스케일을 걷어낸
                // 로컬 축 기준 오프셋을 월드 단위 그대로 넘긴다.
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float2 center = float2(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13);
                float2 offset = worldPos.xy - center;

                float2 axisX = normalize(float2(unity_ObjectToWorld._m00, unity_ObjectToWorld._m10));
                float2 axisY = normalize(float2(unity_ObjectToWorld._m01, unity_ObjectToWorld._m11));
                o.local = float2(dot(offset, axisX), dot(offset, axisY));

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                // 라운드 사각형 SDF
                float radius = min(_CornerRadius, min(_HalfSize.x, _HalfSize.y));
                float2 d = abs(i.local) - (_HalfSize.xy - radius);
                float dist = length(max(d, 0.0)) + min(max(d.x, d.y), 0.0) - radius;

                float aa = max(fwidth(dist), 1e-5);
                col.a *= 1.0 - smoothstep(-aa, aa, dist);

                return col;
            }
            ENDCG
        }
    }

    Fallback "Sprites/Default"
}
