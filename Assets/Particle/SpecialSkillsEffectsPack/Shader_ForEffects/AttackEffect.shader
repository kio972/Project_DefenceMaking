Shader "Custom/AttackEffect"
{
		Properties {
			[Header(Main)]
				[Space]
					[HDR]_TintColor("Color",Color) = (1,1,1,1)
					_MainTex ("Main Tex", 2D) = "white" {}
					_ColorFactor("Color Factor", float) = 1
					
			[Header(Render)]
				[Space]
					[Toggle]_ZWrite("ZWrite On/Off", int) = 0
					[Enum(Culling Off,0, Culling Front, 1, Culling Back, 2)]_Culling("Culling",float) = 2
					[Enum(UnityEngine.Rendering.BlendMode)]_BlendSrc("BlendSrc", float) = 1
					[Enum(UnityEngine.Rendering.BlendMode)]_BlendDst("BlendDst", float) = 1
			
		}
			Category{
					Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
					Blend[_BlendSrc][_BlendDst]
					Cull [_Culling]
					ZWrite[_ZWrite]
					Lighting Off

					SubShader {
						Pass{

							CGPROGRAM
							#pragma vertex vert
							#pragma fragment frag
							#pragma multi_compile_particles
							#pragma multi_compile_fog
							#pragma multi_compile_instancing
							
							#pragma shader_feature UNITY_PARTICLE_INSTANCING_ENABLED
							
							#include "UnityCG.cginc"	
							#include "UnityStandardParticleInstancing.cginc"

							sampler2D _MainTex;
							half4 _MainTex_ST;
													
							

							UNITY_INSTANCING_BUFFER_START(data)
								UNITY_DEFINE_INSTANCED_PROP(half4, _TintColor)
							#define _TintColor_arr data							
									UNITY_DEFINE_INSTANCED_PROP(half, _ColorFactor)
								#define _ColorFactor_arr data							
							UNITY_INSTANCING_BUFFER_END(data)							
							
							struct appdata_t {
								float4 vertex : POSITION;
								float3 normal : NORMAL;
								half4 color : COLOR;
								half4 texcoord : TEXCOORD0;
								UNITY_VERTEX_INPUT_INSTANCE_ID
							};

							struct v2f {
								float4 vertex : SV_POSITION;
								half4 color : COLOR;
								half2 maintex : TEXCOORD0;								
								UNITY_FOG_COORDS(1)								
								UNITY_VERTEX_INPUT_INSTANCE_ID
								UNITY_VERTEX_OUTPUT_STEREO
							};

							v2f vert (appdata_t i)
							{
								v2f o;
								UNITY_SETUP_INSTANCE_ID(i);
								UNITY_INITIALIZE_OUTPUT(v2f, o);
								UNITY_TRANSFER_INSTANCE_ID(i, o);
								UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
															
								o.vertex = UnityObjectToClipPos(i.vertex);

								half4 originaltex = i.texcoord;

								
								#ifdef IS_UNITY_PARTICLE_INSTANCING_ENABLED //GPU Rendering
									
										vertInstancingUVs(i.texcoord, o.maintex);
										o.maintex = TRANSFORM_TEX(o.texcoord, _MainTex);
								#else
										o.maintex = TRANSFORM_TEX(i.texcoord, _MainTex);
								#endif
														
								o.color = i.color;

								UNITY_TRANSFER_FOG(o, o.vertex);
								return o;
							}

							half4 frag(v2f i): SV_Target
							{
								UNITY_SETUP_INSTANCE_ID(i);
								UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

								half4 tex = tex2D(_MainTex, i.maintex);

								half mask_a = tex.a * i.color.a * float(UNITY_ACCESS_INSTANCED_PROP(_TintColor_arr, _TintColor).a);
								half4 res = tex * float4(i.color.rgb,1) * float4(UNITY_ACCESS_INSTANCED_PROP(_TintColor_arr, _TintColor).rgb,1) * float(UNITY_ACCESS_INSTANCED_PROP(_ColorFactor_arr, _ColorFactor));
								half alpha = mask_a *  float(UNITY_ACCESS_INSTANCED_PROP(_ColorFactor_arr, _ColorFactor));
								res.a = saturate(pow(alpha, 2.0f));

								UNITY_APPLY_FOG_COLOR(i.fogCoord, res, half4(0, 0, 0, 0));

								return res;
							}
							ENDCG
					}
				}
		}
}