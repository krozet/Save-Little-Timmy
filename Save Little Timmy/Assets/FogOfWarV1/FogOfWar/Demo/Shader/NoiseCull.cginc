#ifndef _NOISECULL
#define _NOISECULL

sampler3D _NoiseTex_Cullmap;

float4 _CullSphereCore; // max Dis  : X > this => No Cull
float _CullSphereSoftEdge; // internal  : X < this => Full Cull
uint _EnableCull;
float CullTest(float3 worldPos,inout float cone){
    
    float ns = tex3D(_NoiseTex_Cullmap,worldPos * 0.1 + _Time.x).r;
    ns = lerp(0.1,0.9,ns);
    float3 viewDir = normalize(worldPos - _WorldSpaceCameraPos);
    float3 center = _CullSphereCore.xyz - normalize(_CullSphereCore.xyz - _WorldSpaceCameraPos) * _CullSphereCore.w * 0.5;
    float VoS = dot(normalize(center - _WorldSpaceCameraPos),viewDir);
    float DisS2E = length(center - _WorldSpaceCameraPos);

    float3 clostPoint = _WorldSpaceCameraPos + (DisS2E / VoS) * viewDir;
    float disToC = length(clostPoint - center);
    
    float overDis = dot(normalize(clostPoint - worldPos), normalize(_WorldSpaceCameraPos - worldPos));
    float cullThreshold = saturate((disToC - _CullSphereSoftEdge) / (_CullSphereCore.w - _CullSphereSoftEdge));
    cone = overDis;
    return (ns - cullThreshold);
    // if(cullV > 0){
    //     if(overDis < 0.5){
    //         discard;
    //         if(disToC < _CullSphereCore.w){
    //             discard;
    //         }
    //         return 1;
    //     }else{
    //         return 1;
    //     }
    // }else{
    //     return 0;

    // }
}


#endif