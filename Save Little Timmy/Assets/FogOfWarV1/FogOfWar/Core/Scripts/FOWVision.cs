using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThirdPart;

public class FOWVision : MonoBehaviour,IVision {
	#region IVision implementation
	public Vector4 getVision{get;}
	public Vector4 getLastVision{get;}
	private Vector2 lastTimeUV;
	public bool NeedCalculate(FogArea fogArea,out Vector4 visionParam,out Vector4 lastVisionParam)
	{
		
		visionParam = new Vector4 ();
		lastVisionParam = new Vector4();
		if(this.enabled == false || !this.gameObject.activeInHierarchy){
			return false;
		}
		Vector3 pos = fogArea.transform.InverseTransformPoint (position);
//		pos = Vector3.ProjectOnPlane (pos, fogArea.transform.up);
		float x = (pos.x / fogArea.areaSize * 2) * 0.5f + 0.5f;
		float y = (pos.z / fogArea.areaSize * 2) * 0.5f + 0.5f;
		Vector2 currentUV = new Vector2 (x, y);
		visionParam = new Vector4 (currentUV.x, currentUV.y, Mathf.Clamp01 (visionDistance / fogArea.areaSize), 0);
		lastVisionParam = new Vector4(lastTimeUV.x,lastTimeUV.y,Mathf.Clamp01 (visionDistance / fogArea.areaSize), 0);
		if ((currentUV - lastTimeUV).magnitude > (1.0f / (float)fogArea.resolution * fogArea.areaSize) * 0.25f) {
			lastTimeUV = currentUV;
			return true;
		} else {
			return true;
		}
	}
	public Vector3 position {
		get {
			return transform.position;
		}
	}
	public float viewDistance {
		get {
			return visionDistance;
		}
	}
	#endregion
	void OnEnable(){

	}

	public float visionDistance = 10;
	void OnDrawGizmosSelected(){
		Gizmos.color = new Color(1.0f,0.8f,0.6f,0.6f);
		for(int i = 0 ; i < 120 ; i ++){
			float l0 = (float)i / 120.0f * Mathf.PI * 2;
			float l1 = ((float)i+1.0f) / 120.0f * Mathf.PI * 2 ;
			float x0 = Mathf.Sin(l0) * visionDistance;
			float z0 = Mathf.Cos(l0) * visionDistance;
			float x1 = Mathf.Sin(l1) * visionDistance;
			float z1 = Mathf.Cos(l1) * visionDistance;
			Gizmos.DrawLine(transform.position + new Vector3(x0,0,z0) ,transform.position + new Vector3(x1,0,z1));
		}
	}

}
