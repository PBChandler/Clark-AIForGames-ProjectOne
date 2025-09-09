Shader "Unlit/RetroTV"
{
    Properties
    {
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}

        // New properties for the transition effect
        _TransitionNoiseTex("Transition Noise Tex", 2D) = "gray" {}
        _TransitionAmount("Transition Amount", Range(0, 1)) = 0.0

        // Existing properties
        _ScanlineIntensity("Scanline Intensity", Range(0, 1)) = 0.1
        _ScanlineSpeed("Scanline Speed", Range(0, 20)) = 10
        _VignetteIntensity("Vignette Intensity", Range(0, 2)) = 1.0
        _BarrelDistortion("Barrel Distortion", Range(-1, 1)) = 0.15
        _NoiseAmount("Noise Amount", Range(0, 1)) = 0.05
        _ChromaticAberration("Chromatic Aberration", Range(0, 0.05)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        Cull Off

        Pass
        {
            Name "RetroTVPass"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Textures
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_TransitionNoiseTex);
            SAMPLER(sampler_TransitionNoiseTex);

            // Material properties
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;

                float _TransitionAmount;

                float _ScanlineIntensity;
                float _ScanlineSpeed;
                float _VignetteIntensity;
                float _BarrelDistortion;
                float _NoiseAmount;
                float _ChromaticAberration;
            CBUFFER_END

            // Helpers
            float random(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            float2 distort(float2 uv)
            {
                float2 toCenter = float2(0.5, 0.5) - uv;
                float dist = dot(toCenter, toCenter);
                return uv + toCenter * dist * _BarrelDistortion;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // --- Transition Effect Logic ---
                // Random vertical glitch offset
                float verticalGlitch = random(float2(_Time.y, 0)) * 2.0 - 1.0;
                float2 transitionUV = i.uv;
                transitionUV.y += verticalGlitch * _TransitionAmount;

                // Apply barrel distortion
                float2 distortedUV = distort(transitionUV);

                // Chromatic Aberration
                float2 r_uv = distort(transitionUV + float2(_ChromaticAberration, 0));
                float2 b_uv = distort(transitionUV - float2(_ChromaticAberration, 0));

                // Sample texture with distortion + aberration
                half r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, r_uv).r;
                half g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV).g;
                half b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, b_uv).b;

                half4 col = half4(r, g, b, 1.0);

                // --- Retro Effects ---
                // Scanlines
                float scanline = sin((i.uv.y + _Time.y * _ScanlineSpeed / 100.0) * 500.0) * _ScanlineIntensity;
                col.rgb -= scanline;

                // Vignette
                float2 vignetteUV = i.uv - 0.5;
                float vignette = 1.0 - dot(vignetteUV, vignetteUV) * _VignetteIntensity;
                col.rgb *= vignette;

                // Static noise
                float noise = (random(i.uv + _Time.y) - 0.5) * _NoiseAmount;
                col.rgb += noise;

                // --- Transition Noise Overlay ---
                half transitionNoise = SAMPLE_TEXTURE2D(_TransitionNoiseTex, sampler_TransitionNoiseTex, i.uv).r;
                col.rgb = lerp(col.rgb, col.rgb + transitionNoise, _TransitionAmount);

                // Kill outside distortion range
                if (distortedUV.x < 0.0 || distortedUV.x > 1.0 || distortedUV.y < 0.0 || distortedUV.y > 1.0)
                {
                    col = float4(0,0,0,1);
                }

                return col;
            }
            ENDHLSL
        }
    }
}
