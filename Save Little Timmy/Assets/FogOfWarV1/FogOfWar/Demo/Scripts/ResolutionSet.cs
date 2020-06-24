using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSet : MonoBehaviour
{
    [Range(0.25f,1.0f)]
    public float defualtResolution = 0.5f;
    public int resolutionX;
    public int resolutionY;

    // Start is called before the first frame update
    void Awake()
    {
        resolutionX = Display.main.systemWidth;
        resolutionY = Display.main.systemHeight;
        SetResolution(defualtResolution);
        
    }


    public void SetResolution(float scale) {
        scale = Mathf.Clamp(scale, 0.25f, 1.0f);
        Screen.SetResolution(Mathf.FloorToInt(resolutionX * scale), Mathf.FloorToInt(resolutionY * scale),true,60);
    }

}
