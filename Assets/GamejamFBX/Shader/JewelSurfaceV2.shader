Shader "Custom/JewelSurfaceV2"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [HDR] _BaseColor("BaseColor", Color) = (1,1,1,1)
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _FresnelPower("Power",float) = 1
        [HDR]_TopColor("TopColor", color) = (1,1,1,1)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;


        fixed4 _BaseColor;
        half _Glossiness;
        half _Metallic;
        float _FresnelPower;
        fixed4 _TopColor;


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldPos;
        };

        float2 Unity_Voronoi_RandomVector_float (float2 UV, float offset)
        {
            float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
            UV = frac(sin(mul(UV, m)));
            return float2(sin(UV.y*offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
        }
        
        float Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity)
        {
            float2 g = floor(UV * CellDensity);
            float2 f = frac(UV * CellDensity);
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
            // Albedo comes from a texture tinted by color
            fixed4 c = _BaseColor;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            
            float Voronoi = Unity_Voronoi_float(IN.uv_MainTex.xy, 2, 3);
            Voronoi = pow(Voronoi, _FresnelPower);
            float3 Effect = saturate(_TopColor.rgb * Voronoi);

            float fresnel = saturate(dot(o.Normal, normalize(IN.viewDir)));
            fresnel = pow(1 - fresnel, _FresnelPower);

            o.Emission = ( Effect) * _TopColor ;

            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
