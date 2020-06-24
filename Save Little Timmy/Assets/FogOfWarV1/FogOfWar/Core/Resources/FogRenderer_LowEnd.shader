Shader "ThirdPart/FogRenderer/LowEnd"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_FogColor ("Fog Color",Color) = (0,0,0,1)
		_Fog_Height("Fog Height" ,Range (0.001,10)) = 5
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent+100"}
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		ZTest Always
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "FOWCore.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3 wldPos : TEXCOORD1;
				float4 scrPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _FogColor;
			half _Fog_Height;
			uniform half _FogAreaOpacity;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.wldPos = mul(unity_ObjectToWorld, v.vertex);
				o.scrPos = ComputeScreenPos(o.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{	
				fixed2 scrUV = i.uv;//i.scrPos.xy / i.scrPos.w;
				// fixed depth = pixelDepth(scrUV);
				// fixed3 wldPos = GetWorldPositionFromDepthValue(scrUV,depth);
				// scrUV = WorldToFogProjectionUV(wldPos);
				fixed shadow = saturate(_FogAreaOpacity - SamplerSmothedFogData(1-scrUV).r);
				
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return half4(_FogColor.rgb,shadow * _FogColor.a);
			}
			ENDCG
		}
	}
}
