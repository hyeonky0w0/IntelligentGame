Shader "Custom/BulhwaEyeOverlay"
{
    Properties
    {
        _EyeTex         ("Eye Texture (눈동자만, 배경 투명)", 2D) = "white" {}
        _EyeOffset      ("Eye UV Offset",   Vector)       = (0,0,0,0)
        _EffectIntensity("Effect Intensity", Range(0,1))  = 0
        _MaxShift       ("Max UV Shift",     Range(0,0.2)) = 0.08
    }
    SubShader
    {
        Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Offset -1, -1

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _EyeTex; float4 _EyeTex_ST;
            float4 _EyeOffset;
            float  _EffectIntensity;
            float  _MaxShift;

            struct a2v { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION;  float2 uv:TEXCOORD0; };

            v2f vert(a2v v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.uv, _EyeTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                float2 offset = clamp(_EyeOffset.xy, -_MaxShift, _MaxShift) * _EffectIntensity;
                fixed4 col    = tex2D(_EyeTex, i.uv + offset);
                col.a        *= _EffectIntensity;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}
