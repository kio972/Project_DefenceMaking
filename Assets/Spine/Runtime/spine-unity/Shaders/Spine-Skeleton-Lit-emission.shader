// - Vertex Lit + ShadowCaster
// - Premultiplied Alpha Blending (Optional straight alpha input)
// - Double-sided, no depth

Shader "Spine/Skeleton Lit Emission" {
	Properties {
		_Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.1
		[NoScaleOffset] _MainTex ("Main Texture", 2D) = "black" {}

		[NoScaleOffset] _NormalTex ("Normal Texture", 2D) = "black" {}
		_NormalBump("Normal Bump", Range(0.01,100)) = 1

		[NoScaleOffset] _EmissionTex("Emission Texture", 2D) = "White"{}
		_Emission ("Emission", Color) = (0, 0, 0)
		[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
		[Toggle(_DOUBLE_SIDED_LIGHTING)] _DoubleSidedLighting("Double-Sided Lighting", Int) = 0
		[MaterialToggle(_LIGHT_AFFECTS_ADDITIVE)] _LightAffectsAdditive("Light Affects Additive", Float) = 0

	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 100

		Pass {
			Name "Normal"

			Tags { "LightMode"="Vertex" "Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent" }

			Ztest Always
			//ZWrite Off
			Cull Off
			Blend One OneMinusSrcAlpha

			CGPROGRAM
			#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
			#pragma shader_feature _ _DOUBLE_SIDED_LIGHTING
			#pragma shader_feature _ _LIGHT_AFFECTS_ADDITIVE
			#pragma shader_feature _ _EMISSION_MAP
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#define FORWARD_BASE_PASS

			#pragma multi_compile __ POINT SPOT
			#include "CGIncludes/Spine-Skeleton-Lit-Custom.cginc"
			ENDCG

	 	}

		Pass {
			Name "Caster"
			Tags { "LightMode"="ShadowCaster" }
			Offset 1, 1

			Fog { Mode Off }
			ZWrite On
			ZTest LEqual
			Cull Off
			Lighting Off

			CGPROGRAM
			#pragma vertex vertShadow
			#pragma fragment fragShadow
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest

			#define SHADOW_CUTOFF _Cutoff
			#include "CGIncludes/Spine-Skeleton-Lit-Common-Shadow.cginc"

			ENDCG
		}
	}
}
