Shader "Custom/FogOfWar"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _FogColor("Fog Color", Color) = (0,0,0,0.5)
        _PlayerPos("Player Position", Vector) = (0,0,0)
        _RevealRadius("Reveal Radius", Float) = 5.0
    }
        SubShader
        {
            Tags { "RenderPipeline" = "UniversalRenderPipeline" }
            Pass
            {
                Name "ForwardLit"
                Tags { "LightMode" = "UniversalForward" }

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

                struct Attributes
                {
                    float4 positionOS   : POSITION;
                    float2 uv           : TEXCOORD0;
                };

                struct Varyings
                {
                    float4 positionHCS  : SV_POSITION;
                    float2 uv           : TEXCOORD0;
                };

                TEXTURE2D(_BaseMap);
                SAMPLER(sampler_BaseMap);

                float4 _FogColor;
                float4 _PlayerPos;
                float _RevealRadius;

                Varyings vert(Attributes input)
                {
                    Varyings output;
                    output.positionHCS = TransformObjectToHClip(float3(input.positionOS.xyz));
                    output.uv = input.uv;
                    return output;
                }

                half4 frag(Varyings input) : SV_Target
                {
                    half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);

                    // World position of the fragment
                    float2 worldPos = input.positionHCS.xy / input.positionHCS.w;
                    float dist = distance(worldPos, _PlayerPos.xy);

                    // Apply fog based on distance
                    if (dist > _RevealRadius)
                    {
                        baseColor = lerp(baseColor, _FogColor, 1.0 - smoothstep(_RevealRadius - 1.0, _RevealRadius, dist));
                    }

                    return baseColor;
                }
                ENDHLSL
            }
        }
            FallBack "Hidden/Universal Render Pipeline/FallbackError"
}