Shader "Custom/LavaShader"
{
    Properties
    {
        [HDR]_BaseColor ("BaseColor", Color) = (1,1,1,1)
        [HDR]_RippleColor ("RippleColor", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _FlowTex ("FlowArea", 2D) = "white" {}
        _NormalTex ("Normal Map", 2D) = "white" {}
        _BumpScale("Normal Bump", float) = 1
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _RippleSpeed("Speed", float) = 1
        _RippleDensity("Density", float) = 5
        _RipplePower("Power", float) = 5
    }
    SubShader
    {
        Tags { "LightMode" = "ForwardBase" "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _FlowTex;
        sampler2D _NormalTex;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_FlowTex;
            float2 uv_NormalTex;
        };
        half _BumpScale;
        half _Glossiness;
        half _Metallic;
        fixed4 _BaseColor;
        fixed4 _RippleColor;
        half _RippleSpeed;
        half _RippleDensity;
        half _RipplePower;


        UNITY_INSTANCING_BUFFER_START(Props)
            
        UNITY_INSTANCING_BUFFER_END(Props)

        float2 Unity_Voronoi_RandomVector_float (float2 UV, float offset)
        {
            float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
            UV = frac(sin(mul(UV, m)));
            return float2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
        }
        
        float Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity)
        {
            float2 g = floor(UV * CellDensity);
            float2 f = frac(UV * CellDensity);
            float t = 8.0;
            float3 res = float3(8.0, 0.0, 0.0);
        
            for(int y=-1; y<=1; y++)
            {
                for(int x=-1; x<=1; x++)
                {
                    float2 lattice = float2(x,y);
                    float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
                    float d = distance(lattice + offset, f);
        
                    if(d < res.x)
                    {
                        res = float3(d, offset.x, offset.y);
                        
                    }
                }
            }
            return res.x;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float Out;
            float Cells;
            float Voronoi = Unity_Voronoi_float(IN.uv_MainTex.xy + float2 (_Time.x, _Time.x), _Time.x * _RippleSpeed, _RippleDensity);
            Voronoi = pow(Voronoi, _RipplePower);

            float3 fieldAlbedo = tex2D(_MainTex, IN.uv_MainTex.xy);

            float3 Effect = saturate(_RippleColor.rgb * Voronoi);
            Effect += _BaseColor.rgb;

            float3 Lavaflow = lerp( fieldAlbedo, Effect, tex2D(_FlowTex, IN.uv_FlowTex.xy).r);

            
            o.Albedo = Lavaflow;
            o.Normal = UnpackScaleNormal(tex2D(_NormalTex, IN.uv_NormalTex.xy), _BumpScale);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Emission = lerp(Effect, 0, tex2D(_FlowTex, IN.uv_FlowTex.xy).a) * 2;
            
        }
        ENDCG
    }
    FallBack "Diffuse"
}
