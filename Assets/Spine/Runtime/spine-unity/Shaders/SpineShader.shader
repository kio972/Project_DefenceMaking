Shader "Custom/SpineShader" {
	Properties{
		_Cutoff("Shadow alpha cutoff", Range(0,1)) = 0.1
		[NoScaleOffset] _MainTex("Main Texture", 2D) = "black" {}
		[NoScaleOffset] _NormalMap("Normals", 2D) = "bump" {}
		_BumpScale("Bump Scale", Float) = 1

		[Gamma] _Metallic("Metallic", Range(0, 1)) = 0
		_Smoothness("Smoothness", Range(0, 1)) = 0.1

		[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
		[HideInInspector] _StencilRef("Stencil Reference", Float) = 1.0
		[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 8 // Set to Always as default

	}

		SubShader{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }

			Fog { Mode Off }
			Cull Off
			Ztest Always
			//ZWrite Off
			Blend One OneMinusSrcAlpha
			Lighting Off

			Stencil {
				Ref[_StencilRef]
				Comp[_StencilComp]
				Pass Keep
			}

			Pass {
				Name "Normal"

				CGPROGRAM
				#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityPBSLighting.cginc"
				#include "AutoLight.cginc"
				#include "CGIncludes/Spine-Common.cginc"
				sampler2D _MainTex;
				sampler2D _NormalMap;
				float _BumpScale;
				float _Metallic;
				float _Smoothness;

				struct VertexInput {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float3 normal : NORMAL;
					float4 vertexColor : COLOR;
				};

				struct VertexOutput {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float4 vertexColor : COLOR;
					float3 normal : TEXCOORD1;
					float4 worldPos : TEXCOORD2;
				};

				void InitializeFragmentNormal(inout VertexOutput i) {
					i.normal = UnpackScaleNormal(tex2D(_NormalMap, i.uv), _BumpScale);
					i.normal = i.normal.xzy;
				}


				VertexOutput vert(VertexInput v) {
					VertexOutput o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.normal = UnityObjectToWorldNormal(v.normal);
					o.vertexColor = PMAGammaToTargetSpace(v.vertexColor);
					o.worldPos = mul(unity_ObjectToWorld, v.vertex);
					return o;
				}

				float4 frag(VertexOutput i) : SV_Target {
					InitializeFragmentNormal(i);
					float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
					float4 texColor = tex2D(_MainTex, i.uv);
					#if defined(_STRAIGHT_ALPHA_INPUT)
					texColor.rgb *= texColor.a;
					#endif

					return (texColor * i.vertexColor);
				}
				ENDCG
			}

			Pass {
				Name "Caster"
				Tags { "LightMode" = "ShadowCaster" }
				Offset 1, 1
				ZWrite On
				ZTest LEqual

				Fog { Mode Off }
				Cull Off
				Lighting Off

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_shadowcaster
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
				sampler2D _MainTex;
				fixed _Cutoff;

				struct VertexOutput {
					V2F_SHADOW_CASTER;
					float4 uvAndAlpha : TEXCOORD1;
				};

				VertexOutput vert(appdata_base v, float4 vertexColor : COLOR) {
					VertexOutput o;
					o.uvAndAlpha = v.texcoord;
					o.uvAndAlpha.a = vertexColor.a;
					TRANSFER_SHADOW_CASTER(o)
					return o;
				}

				float4 frag(VertexOutput i) : SV_Target {
					fixed4 texcol = tex2D(_MainTex, i.uvAndAlpha.xy);
					clip(texcol.a* i.uvAndAlpha.a - _Cutoff);
					SHADOW_CASTER_FRAGMENT(i)
				}
				ENDCG
			}
		}
			CustomEditor "SpineShaderWithOutlineGUI"
}
