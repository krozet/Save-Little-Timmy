using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThirdPart;

public class FOWOccluder : MonoBehaviour,IOccluder {
	/// <summary>
	/// Occlude Radius in world
	/// </summary>
	[SerializeField]
	private float _radius = 1;
	#region IOccluder implementation
	
	public float radius {
		get {
			return _radius;
		}
	}

	#endregion

	#region IFOWUnit implementation

	public Vector3 position {
		get {
			return transform.position;
		}
	}

	#endregion
	#region  ------------------------Optimize Here if want cutomize-------------------------
	int enterCount;
	public void OnTriggerEnter(Collider other){
		if (other.GetComponent<FOWVision> ()!=null) {
			enterCount++;
		}
		if (enterCount > 0) {
			this.enabled = false;
		} else {
			this.enabled = true;
		}
	}
	public void OnTriggerExit(Collider other){
		if (other.GetComponent<FOWVision> ()!=null) {
			enterCount--;
		}
		if (enterCount > 0) {
			this.enabled = false;
		} else {
			this.enabled = true;
		}
	}
	#endregion
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
