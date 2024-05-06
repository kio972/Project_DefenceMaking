Shader "UI/CustomDissolve"
{
    Properties
    {
        [HideInInspector] [MainTexture] _MainTex("Sprite Texture", 2D) = "white" {}
        [HDR] _Color("Edge Color", Color) = (1, 1, 1, 1)
        [HideInInspector] _HSV("HSV", Color) = (0,0,0,0)
        _DissolveTex("Dissolve Texture", 2D) = "white" {}
        _DissolvePower("Dissolve Power", Range(0, 1)) = 0 
        [HideInInspector] _Switch("Switch", Float) = 0

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }

        SubShader
        {
            Tags
            {
                //"Queue" = "Opaque"
                "IgnoreProjector" = "True"
                "RenderType" = "Opaque"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Stencil
            {
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }

            Cull Off
            Lighting Off
            ZWrite Off
            ZTest[unity_GUIZTestMode]
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask[_ColorMask]

            Pass
            {
                Name "Default"
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0

                #include "UnityCG.cginc"
                #include "UnityUI.cginc"

                #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
                #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    float2 texcoord  : TEXCOORD0;
                    float4 worldPosition : TEXCOORD1;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                sampler2D _MainTex;
                sampler2D _DissolveTex;
                fixed4 _Color;
                float4 _HSV;
                fixed4 _TextureSampleAdd;
                float4 _ClipRect;
                float4 _MainTex_ST;
                float4 _DissolveTex_ST;
                float _DissolvePower;
                fixed _Switch;

                float3 Unity_ColorspaceConversion_RGB_RGB_float(float3 In)
                {
                    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                    float3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
                    return In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
                }

                v2f vert(appdata_t v)
                {
                    v2f OUT;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                    OUT.worldPosition = v.vertex;
                    OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                    OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                    //OUT.color = v.color * _Color;
                    return OUT;
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    half4 color = tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd;

                    half4 dissolve = tex2D(_DissolveTex, IN.texcoord);


                    half edgeWidth = lerp(0,0.2,_Switch);
                    half mask = dissolve.r;

                    half stepValue = (mask + lerp(dissolve.b, dissolve.g, _Switch))/2;

                    half AlphaRange = lerp(1, -edgeWidth, _DissolvePower);
                    half MaskRange = lerp(edgeWidth + 1, 0, _DissolvePower);


                    half Out_Alpha = step(AlphaRange, stepValue);
                    half Out_EdgeMask = Out_Alpha - step(MaskRange, stepValue);


                    float3 EdgeColorHSV = lerp(_HSV.xyz, float3(_HSV.x + 1 / 6, _HSV.y, 0), smoothstep(AlphaRange-0.2, MaskRange -0.05, stepValue));
                    
                    float3 EdgeColor = Unity_ColorspaceConversion_RGB_RGB_float(EdgeColorHSV);

                    color.a *= Out_Alpha;
                    color.rgb = lerp(color.rgb, EdgeColor, Out_EdgeMask);
                    

                    #ifdef UNITY_UI_ALPHACLIP
                    clip(color.a - 0.0001);
                    #endif

                    return color;
                }
            ENDCG
            }
        }
}