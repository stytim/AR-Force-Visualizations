// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "ForceVis/LinearGauge"
{
    Properties
    {
        _FillColor("Fill Color", Color) = (1,1,1,1)
        _DefaultColor("Default Color", Color) = (0,0,0,1)
        _FillRate("Fill Rate", Range(0,1)) = 0.5
        _DangerHeight("Threshold Height", Range(0,0.5)) = 0.175
        _WarningHeight("Warning Threshold Height", Range(0,0.3)) = 0.1
        _WarningColor("Warning Outline Color", Color) = (0,0,0,1) 
        _DangerColor("Danger Outline Color", Color) = (0,0,0,1) 
        _OutlineThickness("Thickness", Range(0,0.1)) = 0.005
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        //Tags { "RenderType"="Opaque" }
        Tags { "RenderType"="Transparent" }
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        //ZTest Always
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade

        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        sampler2D _MainTex;
        fixed4 _FillColor;
        fixed4 _DefaultColor;
        float _FillRate;
        float _DangerHeight;
        float _WarningHeight;
        fixed4 _DangerColor;
        fixed4 _WarningColor;
        float _OutlineThickness;
        half _Glossiness;
        half _Metallic;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {

            float4x4 worldToObject = unity_WorldToObject;
            float3 localPos = mul(worldToObject, float4(IN.worldPos, 1)).xyz;

            float normalizedHeight = localPos.y;

            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            if (normalizedHeight <= _FillRate * 0.15)
            {
                c *= _FillColor;
            }
            else
            {
                c *= _DefaultColor;
            }

            // Calculate distance from threshold height
            float heightDiff = abs(normalizedHeight - _DangerHeight);

            if (heightDiff <= _OutlineThickness)
            {
                c = _DangerColor;
            }

            heightDiff = abs(normalizedHeight - _WarningHeight);

            if (heightDiff <= _OutlineThickness)
            {
                c = _WarningColor;
            }

            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
