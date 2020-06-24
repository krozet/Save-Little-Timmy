using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ThirdPart;
public class DebugFow : MonoBehaviour
{
    public FogArea fogOfWar;
    public RawImage target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        target.texture = fogOfWar.ObstacleMap;
    }
}
