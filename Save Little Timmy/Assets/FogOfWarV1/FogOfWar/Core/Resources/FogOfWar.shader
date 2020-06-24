Shader "Hidden/FogOfWar"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
        Cull Off ZWrite Off ZTest Always
		// Pass 0 : render Shadow Map
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work

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
				float4 vertex : SV_POSITION;
			};
			sampler2D _MainTex;
			float4 _MainTex_ST;
			int _texelSize;
			// uniform sampler2D _fogMapBuffer;
			uniform StructuredBuffer<float4> _VisionData : register(t1);
			int _VisionCount;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			// R chanel , 1 ： in Shadow , 0 is visable
			float frag (v2f i) : SV_Target
			{
				int vb = 0;
				UNITY_LOOP
				for(int s = 0 ; s < _ClearAreaRequestCount ; s++){
					if(length(_ClearAreaRequest[s] -  i.uv) < _ClearAreaRequest[s].z){
						vb = 1;
						return  1;
					}
				}
				for(int index = 0 ; index < _VisionCount ; index ++){
					if(vb == 0){
						
						if(length(i.uv - _VisionData[index].xy) < _VisionData[index].z){
							int iVB = Visiable(_VisionData[index],i.uv);
							vb |= iVB;
						}
						
					}
				}

				return vb;
			}
			ENDCG
		}
		Pass // pass 1 Clear
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
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
				float4 vertex : SV_POSITION;
			};
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _visionParams;
			float4 _visionParamsPre;
			uniform sampler2D _fogMapBuffer;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			// R chanel , 1 ： in Shadow , 0 is visable
			float frag (v2f i) : SV_Target
			{
				// uint pb = floor(tex2D(_fogMapBuffer,i.uv).r);
				// if(length(i.uv - _visionParams.xy) > _visionParams.z || length(i.uv - _visionParamsPre.xy) > _visionParamsPre.z){
				// 	return 0;
				// }else{
				// 	return pb;
				// }
				return 0;
			}
			ENDCG
		}
		Pass // pass 2 SmothFade
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
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
				float4 vertex : SV_POSITION;
			};


			/// target State
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float4 _MainTex_ST;
			/// last state
			uniform sampler2D _OldFogTexture;
			uniform float _fadeSpeed ;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			// 00 10 20 30 40  
			// 01 11 21 31 41
			// 02 12 22 32 42
			// 03 13 23 33 43
			// 04 14 24 34 44
			//

			// R chanel , 1 ： in Shadow , 0 is visable
			float frag (v2f i) : SV_Target
			{
				float last = tex2D(_OldFogTexture,i.uv).r;
				float current = tex2D(_MainTex,i.uv).r;// SamplerBlurry(_MainTex,_MainTex_TexelSize,i.uv,2,0.5).r > 0.5 ? 1 : 0;
				float vec = (current - last) * 0.75 * _fadeSpeed;
				return lerp(last,current, saturate(_fadeSpeed + vec));

			}
			ENDCG
		}
		Pass // pass 3 Blurry Map
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
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
				float4 vertex : SV_POSITION;
			};
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float4 _MainTex_ST;
			float _Blurry;
			float2 _Kernal;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			// R chanel , 1 ： in Shadow , 0 is visable
			float frag (v2f i) : SV_Target
			{

				float alpha = 0;
				float scale = lerp(1,0.5, SamplerDynamicOcclude(i.uv));
				if(_Blurry > 0){
					alpha = SamplerBlurry(_MainTex,_MainTex_TexelSize,i.uv, 3 ,_Blurry,_Kernal * scale);
				}else{
					alpha = tex2D(_MainTex,i.uv);
				}
				return alpha;
			}
			ENDCG
		}
		Pass // pass 4 Copy To RT
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
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
				float4 vertex : SV_POSITION;
			};


			/// target State
			sampler2D _MainTex;
			float4 _MainTex_ST;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}


			// R chanel , 1 ： in Shadow , 0 is visable
			float4 frag (v2f i) : SV_Target
			{
				float2 uv = i.uv;
				float alpha = tex2D(_MainTex,uv).r;
				return float4(1,1,1,1-alpha);
			}
			ENDCG
		}
		Pass // Blurry Upscale
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
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
				float4 vertex : SV_POSITION;
			};


			/// target State
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}


			// R chanel , 1 ： in Shadow , 0 is visable
			float4 frag (v2f i) : SV_Target
			{
				// return tex2D(_MainTex,i.uv);
				return SamplerBlurry(_MainTex,_MainTex_TexelSize,i.uv,1,1).r > 0.8 ? 1 : 0;
			}
			ENDCG
		}
	}
}
