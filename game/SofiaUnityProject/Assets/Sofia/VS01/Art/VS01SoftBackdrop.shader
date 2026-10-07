Shader "SOFIA/SoftBackdrop"
{
    Properties { _BaseMap ("Painting", 2D) = "white" {} _BaseColor ("Tint", Color) = (1,1,1,1) }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent-100" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; };
            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            half4 _BaseColor;
            Varyings vert(Attributes input)
            { Varyings output; output.positionHCS = TransformObjectToHClip(input.positionOS.xyz); output.uv = input.uv; return output; }
            half4 frag(Varyings input) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                half edge = smoothstep(0, .055, input.uv.x) * smoothstep(0, .055, 1 - input.uv.x);
                col.a *= edge;
                return col;
            }
            ENDHLSL
        }
    }
}
