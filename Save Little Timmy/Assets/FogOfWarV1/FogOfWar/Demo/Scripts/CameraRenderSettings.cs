using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera)),DisallowMultipleComponent,ExecuteInEditMode]
public class CameraRenderSettings : MonoBehaviour
{
    private Camera mCam;
    public DepthTextureMode depth;
    // Start is called before the first frame update
    void OnEnable()
    {
        mCam = GetComponent<Camera>();
        mCam.depthTextureMode = depth;
    }
}
