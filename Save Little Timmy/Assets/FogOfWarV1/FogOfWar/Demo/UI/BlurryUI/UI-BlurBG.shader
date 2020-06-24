// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "UI/BlurBG"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Blurry ("Blurry", range(0.0,5)) = 1
        _SamplePixel ("SamplePixel", Range(2,10)) = 8
        _MipmapBlurry ("_MipmapBlurry", range(0.0,1)) = 0.6

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4 scrPos : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            fixed _Blurry;
            float _MipmapBlurry;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = v.texcoord;
                OUT.scrPos = ComputeScreenPos(OUT.vertex);

                OUT.color = v.color;
                return OUT;
            }

            sampler2D _MainTex;
            sampler2D _CMGrabTexture;
            float4 _CMGrabTexture_TexelSize;
            int _SamplePixel;
            fixed4 Blur(sampler2D source,float2 texelSize,float2 uv,float radius,int ignoreAlpha = 0){

            	fixed weight = 0;
            	fixed4 sampled = 0;
            	for(int x = -_SamplePixel ;x <= _SamplePixel; x ++){
	            	for(int y = -_SamplePixel ;y <= _SamplePixel; y ++){
	            		fixed2 ofst = fixed2(x,y) * texelSize.xy * radius;
	            		float2 mip = (1+radius) * _MipmapBlurry;
	            		fixed2 suv = saturate(uv + ofst * mip);

	            		fixed4 col = tex2Dlod(source,fixed4(suv,mip));
						col.rgb = saturate(col.rgb);
	            		fixed w = (1 - saturate(length(fixed2(x,y)) / (fixed)_SamplePixel)) * lerp(col.a,1,ignoreAlpha);
	            		sampled += col * w;
	            		weight += w;
	            	}
            	}
            	return sampled/weight;
            }
            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif
				if (color.a > 0.001) {
					float3 blur = Blur(_CMGrabTexture, _CMGrabTexture_TexelSize.xy, IN.scrPos.xy / IN.scrPos.w, _Blurry, 1).rgb  * _Color.rgb;
					color.rgb = lerp(blur, color.rgb, color.a);
					color.a = saturate(color.a * 256);
				}
				return color;
            }
        ENDCG
        }
    }
}
