using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThirdPart;

public class ClearArea : MonoBehaviour
{
    public FogArea fogArea;
    private int controllID;
    public float radius = 10;
    // Start is called before the first frame update
    void OnEnable()
    {
        fogArea.ClearArea(transform.position,radius,out controllID);
    }
    void OnDisable(){
        fogArea.RemoveAreaClearRequest(controllID);
    }
    // Update is called once per frame
    void Update()
    {
        fogArea.MotifyClearArea(controllID,transform.position);
    }
    	void OnDrawGizmosSelected(){
		for(int i = 0 ; i < 120 ; i ++){
			float l0 = (float)i / 120.0f * Mathf.PI * 2;
			float l1 = ((float)i+1.0f) / 120.0f * Mathf.PI * 2 ;
			float x0 = Mathf.Sin(l0) * radius;
			float z0 = Mathf.Cos(l0) * radius;
			float x1 = Mathf.Sin(l1) * radius;
			float z1 = Mathf.Cos(l1) * radius;
			Gizmos.DrawLine(transform.position + new Vector3(x0,0,z0) ,transform.position + new Vector3(x1,0,z1));
		}
	}
}
