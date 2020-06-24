using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsLevelTest : MonoBehaviour
{

    public Text text;
    void Start(){
        string divice = SystemInfo.graphicsDeviceName;
       bool csSpt = SystemInfo.supportsComputeShaders;
       int graLvl = SystemInfo.graphicsShaderLevel;
       bool rfRhlfSpt = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RHalf);
       bool rfR8Spt = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8);
       bool rfRintSpt = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RInt);
        text.text = System.String.Format("This Divice({5} [<i>Graphics Level : {0}</i>]) {1} ComputeShader \t"
        +"Support RenderTexture [RHalf : {2}] [R8 : {3}] [RInt : {4}]"
        ,graLvl,csSpt ? "<color=#11ff11>SUPPORT</color>" : "<color=#ff1111>UNSUPPORT</color>",rfRhlfSpt,rfR8Spt,rfRintSpt,divice);

   }
}
