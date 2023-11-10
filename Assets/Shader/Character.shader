Shader "Custom/Character"
{
        Properties {
            _MainTex ("Albedo", 2D) = "white" {}
            _Tint ("Tint", Color) = (1, 1, 1, 1)
            
            [NoScaleOffset] _NormalMap ("Normals", 2D) = "bump" {}
            _BumpScale ("Bump Scale", Float) = 1

            [NoScaleOffset] _MetallicMap ("Metallic", 2D) = "white" {}
            [Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
            _Smoothness ("Smoothness", Range(0, 1)) = 0.1
            
            [NoScaleOffset] _EmissionMap ("Emission", 2D) = "black" {}
            _Emission ("Emission", Color) = (0, 0, 0)

            _AlphaCutoff("Alpha Cutoff", Range(0, 1)) = 0.5

            [HideInInspector] _SrcBlend("_SrcBlend", Float) = 1
            [HideInInspector] _DstBlend("_DstBlend", Float) = 0
            [HideInInspector] _ZWrite("_ZWrite", Float) = 1

            _InDirectLightingPower("In Direct Lighting Power", Range(0.0001, 1)) = 1
            _RedCorrection("Red Correction", Range(-0.1, 0.1)) = 0
            _GreenCorrection("Green Correction", Range(-0.1, 0.1)) = 0
            _BlueCorrection("Blue Correction", Range(-0.2, 0.2)) = 0


        }
        
        CGINCLUDE

        #define BINORMAL_PER_FRAGMENT

        ENDCG

    	SubShader {

		    Pass {
                    Tags { "LightMode" = "ForwardBase" "Queue" = "Transparent"}
                    Ztest Always
                    Cull Off
                    Blend[_SrcBlend][_DstBlend]
                    ZWrite[_ZWrite]

                    CGPROGRAM

                    #pragma target 3.0

                    #pragma shader_feature _ _RENDERING_CUTOUT _RENDERING_FADE _RENDERING_TRANSPARENT
	                #pragma shader_feature _METALLIC_MAP
	                #pragma shader_feature _ _SMOOTHNESS_ALBEDO _SMOOTHNESS_METALLIC
	                #pragma shader_feature _NORMAL_MAP
	                #pragma shader_feature _EMISSION_MAP

                    #pragma vertex VertexProgram
                    #pragma fragment FragmentProgram

                    #define FORWARD_BASE_PASS

                    #include "CharacterUse.cginc"

                    ENDCG
           }


       }
       CustomEditor "MonsterShaderGUI"
}
