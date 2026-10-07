Shader "SOFIA/FlowWater"
{
    Properties { [PerRendererData] _MainTex ("Waterfall", 2D) = "white" {} _Color ("Tint", Color) = (1,1,1,1) }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; half4 color : COLOR; };
            struct Varyings { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; half4 color : COLOR; };
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            half4 _Color;
            Varyings vert(Attributes i)
            { Varyings o; o.positionHCS = TransformObjectToHClip(i.positionOS.xyz); o.uv = i.uv; o.color = i.color; return o; }
            half4 frag(Varyings i) : SV_Target
            {
                float2 uv = i.uv;
                uv.x += sin(_Time.y * 1.7 + uv.y * 19.0) * .007;
                uv.y += sin(_Time.y * 2.2 + uv.x * 14.0) * .005;
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * i.color * _Color;
                c.a *= .84 + .13 * sin(_Time.y * 2.9 + uv.y * 25.0);
                return c;
            }
            ENDHLSL
        }
    }
}
