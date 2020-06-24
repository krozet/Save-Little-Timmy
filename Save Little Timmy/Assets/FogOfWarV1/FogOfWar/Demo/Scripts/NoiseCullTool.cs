using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NoiseCullTool : MonoBehaviour
{
    public Texture3D source;
    public float cullRange = 1.5f;
    public float softRange = 0.5f;

    Vector3 pos {
        get{
            return transform.position;
        }
    }
    // Update is called once per frame
    void Update()
    {
        Vector4 vec =new Vector4(pos.x,pos.y,pos.z,cullRange);
        Shader.SetGlobalTexture("_NoiseTex_Cullmap",source);
        Shader.SetGlobalVector("_CullSphereCore",vec);
        Shader.SetGlobalFloat("_CullSphereSoftEdge",softRange);
        Shader.SetGlobalFloat("_EnableCull",1);
    }
    void OnDrawGizmos(){
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position,cullRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,softRange);
    }
}
