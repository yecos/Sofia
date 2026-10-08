Shader "SOFIA/SpriteCleanup"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderPipeline" = "UniversalPipeline"
            "UniversalMaterialType" = "Unlit"
        }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color * _Color;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float4 sampleColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                float4 color = sampleColor * input.color;
                float redEdge = saturate((color.r - color.g - .30) * 7.0)
                    * saturate((color.r - color.b - .28) * 6.0)
                    * saturate((color.r - .58) * 4.0)
                    * saturate((.32 - color.g) * 4.0);
                float yellowEdge = saturate((min(color.r, color.g) - .66) * 5.0)
                    * saturate((.24 - color.b) * 8.0)
                    * saturate((color.r - color.b - .38) * 4.0);
                float greenEdge = saturate((color.g - color.r - .28) * 7.0)
                    * saturate((color.g - color.b - .25) * 6.0)
                    * saturate((color.g - .62) * 4.0);
                color.a *= 1.0 - saturate(max(redEdge, max(yellowEdge, greenEdge)));
                clip(color.a - .012);
                return color;
            }
            ENDHLSL
        }
    }
}
