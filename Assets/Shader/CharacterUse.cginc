#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

sampler2D _MainTex;
float4 _Tint;
float4 _MainTex_ST;
sampler2D _MetallicMap;
float _Metallic;
float _Smoothness;

sampler2D _NormalMap;
float _BumpScale;
sampler2D _EmissionMap;
float3 _Emission;

float _Blend;
float _InDirectLightingPower;
float _RedCorrection;
float _GreenCorrection;
float _BlueCorrection;


struct VertexData {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 tangent : TANGENT;
    float2 uv : TEXCOORD0;
	float4 color : COLOR;
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
	float4 vertexLightColor : TEXCOORD6;
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

float3 GetAlbedo (v2f iP) {
	float3 albedo = tex2D(_MainTex, iP.uv.xy).rgb * _Tint.rgb;
	return albedo;
}

float3 GetTangentSpaceNormal (v2f iP) {
	float3 normal = float3(0, 0, 1);
	#if defined(_NORMAL_MAP)
		normal = UnpackScaleNormal(tex2D(_NormalMap, iP.uv.xy), _BumpScale);
	#endif
	return normal;
}

float GetAlpha (v2f iP) {
	float alpha = _Tint.a;
	#if !defined(_SMOOTHNESS_ALBEDO)
		alpha *= tex2D(_MainTex, iP.uv.xy).a * iP.vertexLightColor.a;
	#endif
	return alpha;
}

UnityLight CreateLight(v2f iP){
    UnityLight light;
	light.dir = 0;
    light.color = 0;
    light.ndotl = 0;

    return light;
}

UnityIndirect CreateIndirectLight (v2f iP, float3 viewDir) {
	UnityIndirect indirectLight;
	indirectLight.diffuse = 0;
	indirectLight.specular = 0;

    #if defined(FORWARD_BASE_PASS)
	indirectLight.diffuse += max(0, ShadeSH9(float4(iP.normal, 1))) * _InDirectLightingPower;
	#endif
	
	return indirectLight;
}

v2f VertexProgram (VertexData v){
    v2f iP;
    iP.pos = UnityObjectToClipPos(v.vertex);
	iP.normal = UnityObjectToWorldNormal(v.normal);
	iP.worldPos = mul(unity_ObjectToWorld, v.vertex);
	iP.vertexLightColor = v.color;
    iP.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);

    return iP;
}

float4 FragmentProgram(v2f iP) : SV_TARGET{
	#if defined(_NORMAL_MAP)
		iP.normal = UnpackScaleNormal(tex2D(_NormalMap, iP.uv.xy), _BumpScale);
	#endif

	iP.normal *= sign(iP.normal.z);

    float3 viewDir = normalize(_WorldSpaceCameraPos - iP.worldPos);
    float3 specularColor;
    float oneMinusReflectivity;
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
	color.a = GetAlpha(iP);

	return color;
}
