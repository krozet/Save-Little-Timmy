Shader "Unlit/test"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Mip ("Mip",Range(0,1)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

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
			};

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float4 _MainTex_ST;
			uniform half4 _visionData;
			uniform uint _texelSize;
			StructuredBuffer<float> _FogPixelBuffer;
			half _Mip;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			fixed4 frag (v2f i) : SV_Target
			{
				int uX = floor(i.uv.x * 128);
				int uY = floor(i.uv.y * 128);
				int index = (uY)*_MainTex_TexelSize.z + uX;
				return _FogPixelBuffer[index];
				fixed4 col = SamplerDynamicOcclude(i.uv) ;
	
				return col;
			}
			ENDCG
		}
	}
}
