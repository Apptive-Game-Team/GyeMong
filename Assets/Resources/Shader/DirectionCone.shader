Shader "Custom/DirectionCone"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,0,0.5)
        _Range ("Range", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 localPos : TEXCOORD0;
            };

            float4 _Color;
            float _Range;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.localPos = v.vertex.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = saturate(i.localPos.y / _Range);

                float alpha = smoothstep(1.0, 0.0, t);

                return float4(_Color.rgb, _Color.a * alpha);
            }
            ENDCG
        }
    }
}
