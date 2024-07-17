Shader "ForceVis/HeatMap"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _MainTex ("Albedo (RGB)", 2D) = "white" {}


        _NormailizedForceMagnitude("Normailized Force Magnitude", Range(0,1)) = 0.5
        _DangerRadius("Danger Threshold Radius", Range(0,0.5)) = 0.06 
        _WarningRadius("Warning Threshold Radius", Range(0,0.5)) = 0.03
        _CircleThickness("Circle Thickness", Range(0,0.1)) = 0.01
        _DangerCircleColor("Circle Color", Color) =(0,0,0,1)
        _WarningCircleColor("Circle Color", Color) =(0,0,0,1)
        _HitPosition("Hit Pos", vector) = (0,0,0,0)
        _RingSize("Ring Size", Range(0, 1)) = 0.09
        _OutsideColor("Outside", Color) = (0,0,0,0)
        _FadeColor("Fade Color", Color) =(0,0,0,1)

        _SpotColor("Spot Color", Color) =(0,1,0,1)

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
        
        LOD 200


        //ZTest Always 
        CGPROGRAM

    
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows  vertex:vert //alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0


        struct Input
        {
            float3 Worldpos;
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        sampler2D _MainTex;

        float _NormailizedForceMagnitude;
        float _DangerRadius;
        float _WarningRadius;
        float _CircleThickness;
        float4 _DangerCircleColor;
        float4 _WarningCircleColor;
        float4 _HitPosition;
        float _RingSize;
        float4 _OutsideColor;
        float4 _FadeColor;
        float4 _SpotColor;

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            //o.my_vertpos = v.vertex;
            o.Worldpos = mul(unity_ObjectToWorld, v.vertex).xyz;
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex);
            float3 worldPos = IN.Worldpos;

            float4x4 worldToObject = unity_WorldToObject;
            float3 localPos = mul(worldToObject, worldPos).xyz;

            // Calculate distance in world space (assuming _HitLocal is now in world space)
            float dist = distance(localPos.xyz, _HitPosition.xyz);
    
            // // Determine the blend strength based on distance from the center
            // float blendStrength = smoothstep(_RingSize - 0.03, _RingSize, dist);

            // // Interpolate between the spotlight color and the outside color
            // fixed4 col = lerp(_SpotColor, _OutsideColor, blendStrength);

            // Hard edge blending
            float blendStrength = step(_RingSize, dist);

            // Interpolate between the spotlight color and the outside color
            fixed4 col = blendStrength > 0 ? _OutsideColor : _SpotColor;
            

            float outline = 0.001;

            float distToThreshold = abs(dist - _DangerRadius);
            bool isWithinThreshold = distToThreshold < _CircleThickness;

            if (isWithinThreshold)
            {
                // Mix the color to indicate the circle boundary
                col = _DangerCircleColor;//lerp(col, _CircleColor, smoothstep(0, _CircleThickness, distToThreshold));
            }

            distToThreshold = abs(dist - _WarningRadius);
            isWithinThreshold = distToThreshold < _CircleThickness;
            if (isWithinThreshold)
            {
                // Mix the color to indicate the circle boundary
                col = _WarningCircleColor;
            }
           
            o.Albedo =  texColor.rgb * col.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = col.a;
            
        }
        ENDCG
    }
    FallBack "Diffuse"
}
