Shader "Custom/TVNoiseURP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseStrength ("Noise Strength", Range(0,1)) = 0.15
        _LineStrength ("Line Strength", Range(0,1)) = 0.05
        _Speed ("Speed", Float) = 30
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_ST;

            float _NoiseStrength;
            float _LineStrength;
            float _Speed;

            float rand(float2 co)
            {
                return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.positionHCS =
                TransformObjectToHClip(IN.positionOS.xyz);

                OUT.uv =
                TRANSFORM_TEX(IN.uv, _MainTex);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                half4 col =
                SAMPLE_TEXTURE2D(_MainTex,
                sampler_MainTex,
                uv);

                // 노이즈
                float noise =
                rand(float2(
                uv.y * _Time.y * _Speed,
                uv.x));

                col.rgb += noise * _NoiseStrength;

                // 스캔라인
                float scanline =
                sin(uv.y * 500);

                col.rgb += scanline * _LineStrength;

                return col;
            }

            ENDHLSL
        }
    }
}