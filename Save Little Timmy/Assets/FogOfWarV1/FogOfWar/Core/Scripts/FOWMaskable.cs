using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ThirdPart;

public class FOWMaskable : MonoBehaviour,IMaskableUnit {

	public UnityEvent OnVisable;
	public UnityEvent OnMasked;
	private bool mVisible;
	public bool visible {
		get {
			return mVisible;
		}
	}
	public Vector3 position{
		get{ 
			return transform.position;
		}
	}

	private FogArea mFogArea;
	public void SetFogArea (FogArea area)
	{
		if (mFogArea == null) {
			mFogArea = area;
			mFogArea.OnFogMapUpdated += MFogArea_OnFogMapUpdated;
		}
	}
	void OnDestroy(){
		mFogArea.OnFogMapUpdated -= MFogArea_OnFogMapUpdated;
	}
	private bool stateUpdated;
	void MFogArea_OnFogMapUpdated ()
	{
		if (!Application.isPlaying) {
			return;
		}
		if(!this.enabled){
			return;
		}
		int errorCode = 0;
		if (mFogArea.TargetIsVisible (this, out errorCode)) {
			if(mVisible == false || stateUpdated == false){
				OnVisable.Invoke();
				stateUpdated = true;
			}
			mVisible = true;
			// renderer.material.color = Color.red;
			// renderer.material.SetColor("_BaseColor",Color.red);
		} else {
			if(mVisible == true || stateUpdated == false){
				OnMasked.Invoke();
				stateUpdated = true;
			}
			mVisible = false;
			// renderer.material.color = Color.grey;
			// renderer.material.SetColor("_BaseColor",Color.grey);
		}
	}
	// Use this for initialization
	void Start () {
		stateUpdated = false;
	}


}
