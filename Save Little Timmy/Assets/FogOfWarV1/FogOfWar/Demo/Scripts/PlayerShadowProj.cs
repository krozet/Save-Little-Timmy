using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerShadowProj : MonoBehaviour
{
    public Transform SunLight;
    public Transform Player;
    public float distance = 2;
    [Range(0,1)]
    public float aoNess = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(SunLight == null || Player == null){
            return;
        }
        transform.position = Player.position + Vector3.Lerp(SunLight.forward,Vector3.down,aoNess) * -1 * distance;
        transform.rotation = Quaternion.Slerp(SunLight.rotation,Quaternion.LookRotation(Vector3.down,SunLight.up),aoNess);
    }
}
