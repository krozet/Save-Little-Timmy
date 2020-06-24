Shader "Custom/StandardNoiseCull"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [HDR]_EmissionColor ("EmissionColor", Color) = (1,1,1,1)
        _EmissionMap ("Emission", 2D) = "black" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        #include "NoiseCull.cginc"
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _EmissionMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_EmissionMap;
            float3 worldPos;
            float4 screenPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _EmissionColor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Emission = tex2D (_EmissionMap, IN.uv_MainTex) * _EmissionColor;
            float coneAngle = 0;
            float cullTest = CullTest(IN.worldPos,coneAngle);
            if(_EnableCull){
                if(cullTest > 0){
                    if(coneAngle < 0.5){
                        uint px = (uint)(IN.screenPos.x / IN.screenPos.w * _ScreenParams.x) % 2;
                        uint py = (uint)(IN.screenPos.y / IN.screenPos.w * _ScreenParams.y) % 2;
                        if(px | py){
                            discard;
                        }
                    }
                }else{
                    if((cullTest + 0.02)> 0 && coneAngle < 0.5){
                            o.Emission += float3(1,0.5,0) * 5;
                        }
                }
            }

            // if((cullV + 0.1)> 0 ){
            //     o.Emission += float3(1,0.5,0) * (1-culled);
            // }
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
