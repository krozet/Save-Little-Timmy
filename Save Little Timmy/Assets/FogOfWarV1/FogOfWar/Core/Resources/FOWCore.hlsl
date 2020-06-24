#ifndef FOWCore
#define FOWCore
#endif
#pragma target 4.5

#define SmallestMipmapLevel 2

uniform float2 hashseed;
uniform float4x4 _FogAreaWorldToLocal;
uniform float4 _FogAreaProjectionParams;
uniform sampler2D _ObstacleMap;
float4 _ObstacleMap_TexelSize;
uniform sampler2D _calculateMask;
uniform sampler2D _fogTexture;
uniform sampler2D _CameraDepthTexture;
uniform sampler2D _SmothedFogTexture;

uniform int _sourceFogMapPixelSize;
uniform StructuredBuffer<int> _DynamicObstacle;
uniform int _ClearAreaRequestCount;
uniform StructuredBuffer<float3> _ClearAreaRequest;

float2 hash( float2 n )
{
    float x = sin(frac(n.x*321.23523-n.y*345.532+hashseed.x*4135.457213) + n.x * n.y*2334.23+n.x);
    float y = sin(frac(n.x*432.23523-n.y*867.532+hashseed.x*8347.645213) + n.x * n.y*5355.68+n.x);
    return float2(x,y);
}


float pixelDepth(float2 uv){
    return Linear01Depth(tex2D(_CameraDepthTexture, uv).r);
}
float3 GetWorldPositionFromDepthValue(float2 uv,float linearDepth){
    float camPosZ = (_ProjectionParams.z - _ProjectionParams.y) * linearDepth;  
    float height = 2 * camPosZ / unity_CameraProjection._m11;
    float width = _ScreenParams.x / _ScreenParams.y * height;
    float camPosX = width * uv.x - width / 2;
    float camPosY = height * uv.y - height / 2;
    float4 camPos = float4(camPosX, camPosY, camPosZ, 1.0); 
    return mul(unity_CameraToWorld, camPos);
}
float2 WorldToFogProjectionUV(float3 wldPos){
	float3 toLocal = mul(_FogAreaWorldToLocal,float4(wldPos,1)).xyz;
	float2 uv = float2(toLocal.x / _FogAreaProjectionParams.w,toLocal.z / _FogAreaProjectionParams.w);
	return 1-(uv * 0.5 + 0.5);
}



float SamplerSmothedFogData(float2 uv){
	return tex2D(_SmothedFogTexture, uv).r;
}
float SkipLength(int mip,int texelSize){
	// 	0    	1/X
	// 	1		2/X
	//	2		4/X
	//	3		8/X
	//	4		16/X
	//				|-----mipmap Pixel size ----| float of
	return saturate(pow(2,mip) / (float)texelSize);
}
int OutOfBounds(float2 coord){
	return 
	coord.x < 0 || coord.x >1 ||
	coord.y < 0 || coord.y >1;
}
int SamplerDynamicOcclude(float2 uv){
	int x = floor(uv.x * _ObstacleMap_TexelSize.z);
	int y = floor(uv.y * _ObstacleMap_TexelSize.w);
	int index = y *_ObstacleMap_TexelSize.z + x;
	return _DynamicObstacle[index];
}
#define AlphaBaise 0.0
int BinarySearch(sampler2D source,inout float2 pos,float2 dir,int texelSize,int minMipLevel,inout float4  col , inout int CastedMipLevel){
	col = 0;
	uint hit = 0;
	col = tex2Dlod(source,float4(pos,CastedMipLevel,CastedMipLevel));
	hit = 0;
	while(col.a > AlphaBaise){
		CastedMipLevel -= 1;
		col = tex2Dlod(source,float4(pos,CastedMipLevel,CastedMipLevel));
		pos -= SkipLength(CastedMipLevel,texelSize) * dir * 0.5;
		hit = col.a > AlphaBaise ? 1 : 0;
		if(hit){
			if(CastedMipLevel <= minMipLevel){
				hit = 1;
				return hit;
			}
		}

	}

	return hit;
}
#define BoosterTraceCone 32
float4 BoosterTrace(sampler2D source,int texelSize,float2 rayStart,float2 rayDir,int TraceStepCount,float coneAngle,int MipBais,inout float2 hitCoord){
	int mip = SmallestMipmapLevel;
    float4 col = float4(0,0,0,0);
    float2 dir = rayDir;
    float2 ns = hash(dir * rayStart);
    float OPD = SkipLength(0,texelSize); // one pixel distance
    hitCoord = rayStart + OPD * dir;
    for(int s = 1 ; s < TraceStepCount ; s++){
		ns = hash(hash(ns - hashseed.xy) + ns);
		dir = normalize(lerp(dir,dir * s + ns,saturate(coneAngle)*(1 - (float)s / (float)TraceStepCount)));
	    hitCoord += SkipLength(mip,texelSize) * dir  *(1-abs(dot(ns,dir)*0.5));
	    if(OutOfBounds(hitCoord)){
	 	   if(col.a>0){
				col.rgb /= col.a;
			}
	   		return col;
	    }
		if(BinarySearch(source,hitCoord,dir,texelSize,(coneAngle + MipBais) * ((float)s / (float)TraceStepCount) ,col,mip)){
	 	   if(col.a>0){
				col.rgb /= col.a;
			}
			return col;
		}else{
//			hitCoord -= SkipLength(mip,texelSize) * dir *0.5;
//			hitCoord -= dir * OPD;
			mip +=1;
//			mip = SmallestMipmapLevel;

		}
    }
	if(col.a>0){
		col.rgb /= col.a;
		col.a = 1;
	}
	return col;
}
int Visiable(float4 _visionParams, float2 uv){

	float2 dir = normalize(_visionParams.xy - uv);
	int visialbe = 1;
	float2 pos = uv;
	int step = length(_ObstacleMap_TexelSize.zw)*0.5;
	step = floor(clamp(step,8,256));
	UNITY_LOOP
	for(int s = 0 ; s < step ; s++){
		pos = uv + _ObstacleMap_TexelSize.xy * dir * s * 0.5;
		int miss = dot(dir, normalize(_visionParams.xy - pos)) < 0.5 ? 1 : 0;

		if(length(pos - _visionParams.xy) > length(_ObstacleMap_TexelSize.xy) && !miss){
			int ob = tex2D(_ObstacleMap,pos).r > 0 ? 1 : 0;
			int sc = SamplerDynamicOcclude(pos) > 0 ? 1 : 0;
			if((ob + sc) > 0.0){
				visialbe = 0;
				return visialbe;
			}
		}else{
			return visialbe;
		}
	}

	return visialbe;
}
#define _MipmapBlurry 0.5
float4 SamplerBlurry(sampler2D source , float2 texelSize,float2 uv,int _SamplePixel,float radius,float2 kernal = 1){
	float weight = 0;
	float4 sampled = 0;
	// UNITY_LOOP
	// for(int x = -_SamplePixel ;x <= _SamplePixel; x ++){
	// 	UNITY_LOOP
	// 	for(int y = -_SamplePixel ;y <= _SamplePixel; y ++){
	// 		float2 ofst = float2(x,y) * texelSize * radius;
	// 		float2 mip = (radius) * _MipmapBlurry;
	// 		float2 suv = saturate(uv + ofst * mip);
	// 		float4 col = tex2Dlod(source,float4(suv,mip));
	// 		float w = (1 - saturate(length(float2(x,y)) / (float)_SamplePixel)) * col.a;
	// 		sampled += col * w;
	// 		weight += w;
	// 	}
	// }
	float2 ns = hash(uv);
	UNITY_LOOP
	for(int y = -_SamplePixel ;y <= _SamplePixel; y ++){
		float2 ofst = y * texelSize * radius * (kernal );
		float2 mip = (radius) * _MipmapBlurry;
		float2 suv = saturate(uv + ofst * mip);
		float4 col = tex2Dlod(source,float4(suv,mip));
		float w = (1 - saturate(length(float(y)) / (float)_SamplePixel)) * col.a;
		sampled += col * w;
		weight += w;
	}
	return sampled/weight;
}