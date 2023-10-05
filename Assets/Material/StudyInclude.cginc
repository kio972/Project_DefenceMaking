#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

sampler2D _MainTex, _DetailTex, _DetailMask;
float4 _Tint;
float4 _MainTex_ST, _DetailTex_ST;
sampler2D _MetallicMap;
float _Metallic;
float _Smoothness;
//sampler2D _HeightMap;
//float4 _HeightMap_TexelSize;
sampler2D _NormalMap, _DetailNormalMap;
float _BumpScale, _DetailBumpScale;
sampler2D _EmissionMap;
float3 _Emission;
sampler2D _OcclusionMap;
float _OcclusionStrength;
float _Blend;


struct VertexData {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 tangent : TANGENT;
    float2 uv : TEXCOORD0;
};

struct v2f {
    float4 pos : SV_POSITION;
    float4 uv : TEXCOORD0;
	float3 normal : TEXCOORD1;

	#if defined(BINORMAL_PER_FRAGMENT)
		float4 tangent : TEXCOORD2;
	#else
		float3 tangent : TEXCOORD2;
		float3 binormal : TEXCOORD3;
	#endif

	float3 worldPos : TEXCOORD4;

	SHADOW_COORDS(5)

	#if defined(VERTEXLIGHT_ON)
		float3 vertexLightColor : TEXCOORD6;
	#endif
};

float GetMetallic (v2f iP) {
	#if defined(_METALLIC_MAP)
		return tex2D(_MetallicMap, iP.uv.xy).r;
	#else
		return _Metallic;
	#endif
}

float GetSmoothness (v2f iP) {
	float smoothness = 1;
	#if defined(_SMOOTHNESS_ALBEDO)
		smoothness = tex2D(_MainTex, iP.uv.xy).a;
	#elif defined(_SMOOTHNESS_METALLIC) && defined(_METALLIC_MAP)
		smoothness = tex2D(_MetallicMap, iP.uv.xy).a;
	#endif
	return smoothness * _Smoothness;
}

float3 GetEmission (v2f iP) {
	#if defined(FORWARD_BASE_PASS)
		#if defined(_EMISSION_MAP)
			return tex2D(_EmissionMap, iP.uv.xy) * _Emission;
		#else
			return _Emission;
		#endif
	#else
		return 0;
	#endif
}

float GetOcclusion (v2f iP) {
	#if defined(_OCCLUSION_MAP)
		return lerp(1, tex2D(_OcclusionMap, iP.uv.xy).g, _OcclusionStrength);
	#else
		return 1;
	#endif
}

float GetDetailMask (v2f iP) {
	#if defined (_DETAIL_MASK)
		return tex2D(_DetailMask, iP.uv.xy).a;
	#else
		return 1;
	#endif
}

float3 GetAlbedo (v2f iP) {
	float3 albedo = tex2D(_MainTex, iP.uv.xy).rgb * _Tint.rgb;
	#if defined (_DETAIL_ALBEDO_MAP)
		float3 details = tex2D(_DetailTex, iP.uv.zw) * unity_ColorSpaceDouble;
		albedo = lerp(albedo, albedo * details, GetDetailMask(iP));
	#endif
	return albedo;
}

float3 GetTangentSpaceNormal (v2f iP) {
	float3 normal = float3(0, 0, 1);
	#if defined(_NORMAL_MAP)
		normal = UnpackScaleNormal(tex2D(_NormalMap, iP.uv.xy), _BumpScale);
	#endif
	#if defined(_DETAIL_NORMAL_MAP)
		float3 detailNormal = UnpackScaleNormal(tex2D(_DetailNormalMap, iP.uv.zw), _DetailBumpScale);
		detailNormal = lerp(float3(0, 0, 1), detailNormal, GetDetailMask(iP));
		normal = BlendNormals(normal, detailNormal);
	#endif
	return normal;
}

//float GetAlpha (v2f iP) {
//	float alpha = _Tint.a;
//	#if !defined(_SMOOTHNESS_ALBEDO)
//		alpha *= tex2D(_MainTex, iP.uv.xy).a;
//	#endif
//	return alpha;
//}

float3 BoxProjection (float3 direction, float3 position,float4 cubemapPosition, float3 boxMin, float3 boxMax) {
	#if UNITY_SPECCUBE_BOX_PROJECTION
		UNITY_BRANCH
		if (cubemapPosition.w > 0) {
			float3 factors = ((direction > 0 ? boxMax : boxMin) - position) / direction;
			float scalar = min(min(factors.x, factors.y), factors.z);
			direction = direction * scalar + (position - cubemapPosition);
		}
	#endif
	return direction;

}

UnityLight CreateLight(v2f iP){
    UnityLight light;

    #if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
		light.dir = normalize(_WorldSpaceLightPos0.xyz - iP.worldPos);
	#else
		light.dir = _WorldSpaceLightPos0.xyz;
	#endif

	UNITY_LIGHT_ATTENUATION(attenuation, iP, iP.worldPos);

    light.color = _LightColor0.rgb * attenuation;
    light.ndotl = dot(light.dir, iP.normal) * 0.5f + 0.5f;

    return light;
}

UnityIndirect CreateIndirectLight (v2f iP, float3 viewDir) {
	UnityIndirect indirectLight;
	indirectLight.diffuse = 0;
	indirectLight.specular = 0;

	#if defined(VERTEXLIGHT_ON)
		indirectLight.diffuse = iP.vertexLightColor;
	#endif

    #if defined(FORWARD_BASE_PASS)
		indirectLight.diffuse += max(0, ShadeSH9(float4(iP.normal, 1)));
		float3 reflectionDir = reflect(-viewDir, iP.normal);
		Unity_GlossyEnvironmentData envData;
		envData.roughness = 1 - GetSmoothness(iP);
		envData.reflUVW = BoxProjection(
			reflectionDir, iP.worldPos,
			unity_SpecCube0_ProbePosition,
			unity_SpecCube0_BoxMin, unity_SpecCube0_BoxMax
		);
		float3 probe0 = Unity_GlossyEnvironment(
			UNITY_PASS_TEXCUBE(unity_SpecCube0), unity_SpecCube0_HDR, envData
		);
		envData.reflUVW = BoxProjection(
			reflectionDir, iP.worldPos,
			unity_SpecCube1_ProbePosition,
			unity_SpecCube1_BoxMin, unity_SpecCube1_BoxMax
		);
		float interpolator = unity_SpecCube0_BoxMin.w;
		#if UNITY_SPECCUBE_BLENDING
			UNITY_BRANCH
			if (interpolator < 0.99999) {
				float3 probe1 = Unity_GlossyEnvironment(
					UNITY_PASS_TEXCUBE_SAMPLER(unity_SpecCube1, unity_SpecCube0),
					unity_SpecCube0_HDR, envData
				);
				indirectLight.specular = lerp(probe1, probe0, interpolator);
			}
			else {
				indirectLight.specular = probe0;
			}
		#else
			indirectLight.specular = probe0;
		#endif
	float occlusion = GetOcclusion(iP);
	indirectLight.diffuse *= occlusion;
	indirectLight.specular *= occlusion;
	#endif

	return indirectLight;
}

void ComputeVertexLight (inout v2f iP) {
    #if defined(VERTEXLIGHT_ON)
		iP.vertexLightColor = Shade4PointLights(
			unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
			unity_LightColor[0].rgb, unity_LightColor[1].rgb,
			unity_LightColor[2].rgb, unity_LightColor[3].rgb,
			unity_4LightAtten0, iP.worldPos, iP.normal
		);
	#endif
}

float3 CreateBinormal (float3 normal, float3 tangent, float binormalSign) {
	return cross(normal, tangent.xyz) * (binormalSign * unity_WorldTransformParams.w);
}


void InitializeFragmentNormal(inout v2f iP) {
    //Height map
	//float2 du = float2(_HeightMap_TexelSize.x * 0.5, 0);
	//float u1 = tex2D(_HeightMap, iP.uv + du);
	//float u2 = tex2D(_HeightMap, iP.uv - du);

	//float2 dv = float2(0, _HeightMap_TexelSize.y * 0.5);
	//float v1 = tex2D(_HeightMap, iP.uv + dv);
	//float v2 = tex2D(_HeightMap, iP.uv - dv);
    
    //iP.normal = float3(u1 - u2, 1, v1 - v2);
	//float3 mainNormal = UnpackScaleNormal(tex2D(_NormalMap, iP.uv.xy), _BumpScale);
 //   float3 detailNormal = UnpackScaleNormal(tex2D(_DetailNormalMap, iP.uv.zw), _DetailBumpScale);
	//detailNormal = lerp(float3(0, 0, 1), detailNormal, GetDetailMask(iP));
	float3 tangentSpaceNormal = GetTangentSpaceNormal(iP);

	#if defined(BINORMAL_PER_FRAGMENT)
		float3 binormal = CreateBinormal(iP.normal, iP.tangent.xyz, iP.tangent.w);
	#else
		float3 binormal = iP.binormal;
	#endif

    iP.normal = normalize(
		tangentSpaceNormal.x * iP.tangent +
		tangentSpaceNormal.y * binormal +
		tangentSpaceNormal.z * iP.normal
	);

}

v2f VertexProgram (VertexData v){
    v2f iP;
    iP.pos = UnityObjectToClipPos(v.vertex);
	iP.normal = UnityObjectToWorldNormal(v.normal);
	iP.worldPos = mul(unity_ObjectToWorld, v.vertex);

	#if defined(BINORMAL_PER_FRAGMENT)
		iP.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
	#else
		iP.tangent = UnityObjectToWorldDir(v.tangent.xyz);
		iP.binormal = CreateBinormal(i.normal, i.tangent, v.tangent.w);
	#endif

    iP.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
	iP.uv.zw = TRANSFORM_TEX(v.uv, _DetailTex);

	TRANSFER_SHADOW(iP);

    ComputeVertexLight(iP);
    
    return iP;
}

float4 FragmentProgram (v2f iP) : SV_TARGET {

    InitializeFragmentNormal(iP);

    float3 viewDir = normalize(_WorldSpaceCameraPos - iP.worldPos);
    float3 specularColor;
    float oneMinusReflectivity;
    //albedo *= tex2D(_HeightMap, iP.uv.xy);
    float3 albedo = DiffuseAndSpecularFromMetallic(
		GetAlbedo (iP), 
		GetMetallic(iP), 
		specularColor, 
		oneMinusReflectivity
	);

    float4 color = UNITY_BRDF_PBS(
		albedo, 
		specularColor, 
		oneMinusReflectivity, 
		GetSmoothness(iP), 
		iP.normal, 
		viewDir, 
		CreateLight(iP), 
		CreateIndirectLight(iP, viewDir)
	);
	color.rgb += GetEmission(iP);
	return color;
}
