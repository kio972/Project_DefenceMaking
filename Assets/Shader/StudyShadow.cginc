#if !defined(MY_SHADOWS_INCLUDED)
#define MY_SHADOWS_INCLUDED

#include "UnityCG.cginc"

struct VertexData {
	float4 position : POSITION;
	float3 normal : NORMAL;
};

float4 ShadowVertexProgram (VertexData v) : SV_POSITION {
	float4 position = UnityClipSpaceShadowCasterPos(v.position.xyz, v.normal);
	return UnityApplyLinearShadowBias(position);
}

half4 ShadowFragmentProgram () : SV_TARGET {
	return 0;
}

#endif