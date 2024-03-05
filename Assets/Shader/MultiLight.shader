Shader "Custom/MultiLight"
{
        Properties {
            _MainTex ("Albedo", 2D) = "white" {}
            _Tint ("Tint", Color) = (1, 1, 1, 1)
            
            [NoScaleOffset] _NormalMap ("Normals", 2D) = "bump" {}
            _BumpScale ("Bump Scale", Float) = 1

            [NoScaleOffset] _MetallicMap ("Metallic", 2D) = "white" {}
            [Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
            _Smoothness ("Smoothness", Range(0, 1)) = 0.1
            [NoScaleOffset] _HeightMap ("Heights", 2D) = "gray" {}
            _HeightScale("Bump Scale", Float) = 1

            _DetailTex ("Detail Albedo", 2D) = "gray" {}

            [NoScaleOffset] _DetailNormalMap ("Detail Normals", 2D) = "bump" {}
            _DetailBumpScale ("Detail Bump Scale", Float) = 1
            
            [NoScaleOffset] _EmissionMap ("Emission", 2D) = "black" {}
            _Emission ("Emission", Color) = (0, 0, 0)

            [NoScaleOffset] _OcclusionMap ("Occlusion", 2D) = "white" {}
            _OcclusionStrength("Occlusion Strength", Range(0, 1)) = 1

            [NoScaleOffset] _DetailMask ("Detail Mask", 2D) = "white" {}

            _AlphaCutoff("Alpha Cutoff", Range(0, 1)) = 0.5

            [HideInInspector] _SrcBlend("_SrcBlend", Float) = 1
            [HideInInspector] _DstBlend("_DstBlend", Float) = 0
            [HideInInspector] _ZWrite("_ZWrite", Float) = 1


        }
        
        CGINCLUDE

        #define BINORMAL_PER_FRAGMENT

        ENDCG

    	SubShader {

		    Pass {
                    Tags { "LightMode" = "ForwardBase" }
                    Blend[_SrcBlend][_DstBlend]
                    ZWrite[_ZWrite]

                    CGPROGRAM

                    #pragma target 3.0

                    #pragma shader_feature _ _RENDERING_CUTOUT _RENDERING_FADE _RENDERING_TRANSPARENT
	                #pragma shader_feature _METALLIC_MAP
	                #pragma shader_feature _ _SMOOTHNESS_ALBEDO _SMOOTHNESS_METALLIC
	                #pragma shader_feature _NORMAL_MAP
	                #pragma shader_feature _OCCLUSION_MAP
	                #pragma shader_feature _EMISSION_MAP
	                #pragma shader_feature _DETAIL_MASK
	                #pragma shader_feature _DETAIL_ALBEDO_MAP
	                #pragma shader_feature _DETAIL_NORMAL_MAP

                    #pragma multi_compile _ SHADOWS_SCREEN
                    #pragma multi_compile _ VERTEXLIGHT_ON

                    #pragma vertex VertexProgram
                    #pragma fragment FragmentProgram

                    #define FORWARD_BASE_PASS

                    #include "StudyInclude.cginc"

                    ENDCG
           }

            Pass {
                    Tags { "LightMode" = "ForwardAdd" }

                    Blend[_SrcBlend] One
                    ZWrite off

                    CGPROGRAM

                    #pragma target 3.0

                    #pragma shader_feature _METALLIC_MAP
                    #pragma shader_feature _ _SMOOTHNESS_ALBEDO _SMOOTHNESS_METALLIC
                    #pragma shader_feature _NORMAL_MAP
                    #pragma shader_feature _DETAIL_MASK
                    #pragma shader_feature _DETAIL_ALBEDO_MAP
                    #pragma shader_feature _DETAIL_NORMAL_MAP

                    #pragma multi_compile_fwdadd_fullshadows

                    #pragma vertex VertexProgram
                    #pragma fragment FragmentProgram

                    #include "StudyInclude.cginc"

                    ENDCG
           }

            Pass {
                    Tags { "LightMode" = "ShadowCaster" }

                    CGPROGRAM

                    #pragma target 3.0

                    #pragma vertex ShadowVertexProgram
                    #pragma fragment ShadowFragmentProgram

                    #include "StudyShadow.cginc"

                    ENDCG
            }
       }
       CustomEditor "StudyShaderGUI"
}
