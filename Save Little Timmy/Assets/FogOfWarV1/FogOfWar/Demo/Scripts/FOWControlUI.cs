using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThirdPart;

public class FOWControlUI : MonoBehaviour
{
    public FogArea fogArea;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetResolution(int index){
        var res = (FogArea.Resolution)(Mathf.Pow(2,index) * 64);
        Debug.Log(res.ToString());
        fogArea.resolution = res;
    }
    public void SetFade(float smoth){
        fogArea.smoothFade = smoth;
    }
    public void SetOpacity(float op){
        fogArea.fogAreaOpacity = op;
    }
    public void SetBlurry(float blur){
        fogArea.blurryFogMap = blur;
    }
}
