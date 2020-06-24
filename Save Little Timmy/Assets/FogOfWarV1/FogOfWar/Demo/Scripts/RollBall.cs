using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollBall : MonoBehaviour {
	private Rigidbody rigid;
	public Transform Camera;
	public float force = 2;
	[Range(0,1)]
	public float damping = 0.9f;
	public bool UseMovePosition;
	[Range(0.25f,1f)]
	public float Resolution = 1;
	static int OriScW;
	static int OriScH;

	private Vector3 startPos;
	private Quaternion startRot;
	// Use this for initialization
    void Awake() {

		Application.targetFrameRate = 60;
        //if (OriScW == 0) {
        //    OriScW = Screen.width;
        //    OriScH = Screen.height;
        //}
        //Screen.SetResolution((int)(OriScW * Resolution), (int)(OriScH * Resolution), true, 60);
    }
    void Start () {
		if(Camera == null){
			Camera = UnityEngine.Camera.main.transform;
		}
		rigid = GetComponent<Rigidbody> ();
		touchRecord = new  Dictionary<int, Vector2> ();
		touchStart = new  Dictionary<int, Vector2> ();
		
		startPos = transform.position;
		startRot = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		if(touchRecord == null){
			touchRecord = new Dictionary<int, Vector2>();
			touchStart = new Dictionary<int, Vector2>();
		}
		var touchInput = Vector2.zero;//
		bool jump = false;
		foreach(var touch in touchRecord){
			if(touchStart[touch.Key].x < Screen.width * 0.5f){
				touchInput = touchRecord[touch.Key].normalized * Mathf.Clamp01(touchRecord[touch.Key].magnitude/ (Screen.width * 0.15f));
			}else{
				jump = true;
			}
		}
		if(Input.GetKey(KeyCode.Space)){
			jump = true;
		}
		
		float velc = Mathf.Pow(rigid.velocity.magnitude,2) * 2 + 2.0f;
        JoyStickGraphic.TryGetAxis("left", out touchInput) ;

		float forceLR =  touchInput.x / velc;
		float forceFB =  touchInput.y / velc;
		Vector3 dir = new Vector3 (forceLR, 0, forceFB);
		dir = Camera.TransformDirection (dir);
		dir = Vector3.ProjectOnPlane (dir, Vector3.up) / Mathf.Abs(Vector3.Dot(Camera.forward,Vector3.up));
		float addtive = Mathf.Clamp01(Vector3.Dot (rigid.velocity, dir)) + 0.01f;
		if(UseMovePosition){
			rigid.MovePosition(rigid.position + dir * force *0.5f * Time.deltaTime);// * addtive);
			// rigid.velocity = Vector3.zero;
			rigid.angularVelocity = Vector3.zero;
		}else{
			rigid.AddForce ( dir * force * 10 * Time.deltaTime / addtive);
			// rigid.velocity = Vector3.Lerp(rigid.velocity, ( dir * force * 10 * Time.deltaTime),10 * Time.deltaTime);// * addtive);
		}
		if(jump){
			rigid.AddForce((jumpDir.normalized * rigid.mass + dir) * 200.0f , ForceMode.Force);
		}
		if(dir.magnitude <= 0.01f){
			float gV = Vector3.Dot(Vector3.down,rigid.velocity.normalized);
			rigid.velocity *= Mathf.Lerp(damping,1,gV);
		}
		TouchInput();
		Debug.DrawRay(transform.TransformPoint(rigid.centerOfMass),jumpDir);

		if(transform.position.y < -10 || transform.position.y > 20){
			transform.position = startPos;
			transform.rotation = startRot;
			rigid.velocity = Vector3.zero;
		}
	}

	Dictionary<int,Vector2> touchStart;
	Dictionary<int,Vector2> touchRecord;
	void TouchInput(){
		if(touchRecord == null){
			touchRecord = new Dictionary<int, Vector2>();
			touchStart = new Dictionary<int, Vector2>();
		}
		if(Input.touchCount == 0){
			touchStart.Clear();
			touchRecord.Clear();
		}
		// var vector = new List<Vector2>();
		for(int i=0; i< Input.touchCount; i++){
			var touch = Input.GetTouch(i);
			if(touch.phase == TouchPhase.Began){
				Vector2 ori = touch.position;
				if(touchRecord.TryGetValue(i,out ori)){
					touchRecord.Add(i,Vector2.zero);
				}else{
					touchRecord.Add(i,Vector2.zero);
				}
				if(touchStart.TryGetValue(i,out ori)){
					touchStart[i] = touch.position;
				}else{
					touchStart.Add(i,touch.position);
				}
			}else if(touch.phase == TouchPhase.Moved){
				Vector2 ori = touch.position;
				if(touchRecord.TryGetValue(i,out ori)){
					touchRecord[i] = touch.position - touchStart[i];
				}else{
					touchRecord.Add(i,touch.position - touchStart[i]);
				}
			}else if(touch.phase == TouchPhase.Ended){
				touchRecord.Remove(i);
				touchStart.Remove(i);
			}
		}
		// if(vector.Count == 0){
		// 	vector.Add(Vector2.zero);
		// }
	}
	Vector3 jumpDir;
	List<Collider> touchedCol;
	void OnCollisionEnter(Collision col){
		if(touchedCol == null){
			touchedCol = new List<Collider>();
		}
		if(!touchedCol.Contains(col.collider)){
			touchedCol.Add(col.collider);
		}
		foreach(var item in col.contacts){
			jumpDir += (transform.TransformPoint(rigid.centerOfMass) - item.point);
		}
	}

	void OnCollisionExit(Collision col){
		if(touchedCol == null){
			touchedCol = new List<Collider>();
		}
		if(touchedCol.Contains(col.collider)){
			touchedCol.Remove(col.collider);
		}
		if(touchedCol.Count == 0){
			jumpDir = Vector3.zero;
		}
	}
}
