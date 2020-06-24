using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera)),ExecuteInEditMode]
public class CameraImageExpose : MonoBehaviour
{
    [SerializeField]
    private bool downSample = false;
    private Camera mCam;
    private CommandBuffer copyImgBuffer;
    private int texID;
    public RenderTexture tex;
    public bool safeModel;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        mCam = GetComponent<Camera>();
        texID = Shader.PropertyToID ("_CMGrabTexture");
            int W = downSample ? Screen.width >> 2 : Screen.width;
        int H = downSample ? Screen.height >> 2 : Screen.height;
        tex = RenderTexture.GetTemporary(W, H, 16, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Default);
        tex.useMipMap = true;
        Shader.SetGlobalTexture("_CMGrabTexture",tex);
        if(!safeModel){
            copyImgBuffer = new CommandBuffer();
            copyImgBuffer.name = "Commandbuffer GrabTexture";
            copyImgBuffer.Blit(BuiltinRenderTextureType.CurrentActive,tex);
            mCam.AddCommandBuffer(CameraEvent.BeforeImageEffects,copyImgBuffer);
        }

    }

    // Update is called once per frame
    void OnDisable()
    {

        if(copyImgBuffer != null){
            mCam.RemoveCommandBuffer(CameraEvent.BeforeImageEffects,copyImgBuffer);
            copyImgBuffer.ReleaseTemporaryRT(texID);
            copyImgBuffer.Release();
        }
        if(tex != null){
            RenderTexture.ReleaseTemporary(tex);
        }
    } 

    void OnRenderImage(RenderTexture sour, RenderTexture dest){
        if(safeModel){
            Graphics.Blit(sour,tex);
        }
        Graphics.Blit(sour,dest);
    }  

}
