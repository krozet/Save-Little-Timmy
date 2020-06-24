using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmothFollowHorizontal : MonoBehaviour {
	private Vector3 localPos;
	public Transform follow;
	public float speed = 10;
	[Range(1,10)]
	public float smoth = 5;
	[Range(1f,20f)]
	public float smothRange = 10;
	// Use this for initialization
	void Start () {
		localPos = follow.position - transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 targetPos = follow.position - localPos;
		float slow = Mathf.Clamp01((targetPos - transform.position).magnitude / smothRange);
		slow = Mathf.Pow (slow, smoth);
		transform.position = Vector3.Lerp (transform.position,targetPos , speed * Time.deltaTime * slow);
        Vector3 lookPos = follow.position;// + follow.velocity;
		Quaternion rotation = Quaternion.LookRotation (lookPos - transform.position);
		transform.rotation = Quaternion.Slerp (transform.rotation, rotation, 1 * Time.deltaTime);
	}
}
