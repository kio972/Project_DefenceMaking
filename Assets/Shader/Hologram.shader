// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Hologram"
{
    Properties
    {
        _HologramColor ("HologramColor", Color) = (1,1,1,1)
        [Enum(Off, 0, On, 1)] _Effect ("Effect", Float) = 0 
        _BumpScale("Bump Scale", Range(0,1)) = 1
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}

        CGPROGRAM
        #pragma surface surf Lambert noambient vertex:vert alpha:fade


        float4 _HologramColor;
        float _Effect;
        float _BumpScale;

        struct Input
        {
            float3 viewDir;
            float3 worldPos;
            float3 localPos;

        };


        void vert (inout appdata_full v, out Input o) {
          UNITY_INITIALIZE_OUTPUT(Input,o);
          o.localPos = v.vertex.xyz;
        }


        void surf(Input IN, inout SurfaceOutput o)
        {
            o.Emission = _HologramColor.rgb * _BumpScale;
            o.Alpha = pow(frac((IN.localPos.x * IN.localPos.x + IN.localPos.y * IN.localPos.y) * 4 - _Time.y * _Effect),3);

        }

        ENDCG
    }
    FallBack "Diffuse"
}
