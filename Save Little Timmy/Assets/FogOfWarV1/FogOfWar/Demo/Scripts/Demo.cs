using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThirdPart;

public class Demo : MonoBehaviour
{


    private GameObject selectionSphere;
    private GameObject getSelectionSphere{
        get{
            if(selectionSphere == null){
                selectionSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                selectionSphere.GetComponent<Collider>().enabled = false;
                selectionSphere.transform.localScale = Vector3.one * 0.25f;
                MaterialPropertyBlock matCol = new MaterialPropertyBlock();
                matCol.SetColor("_Color",Color.red);
                selectionSphere.GetComponent<Renderer>().SetPropertyBlock(matCol);
            }
            return selectionSphere;
        }

    }
    public FogArea fogArea;

    private bool addEyeVisionWaiting = false;
    public GameObject visionPrefabe;

    private Dictionary<GameObject,float> visionInstances;
    private GameObject currentVisionInstance;
    public void AddVision(){
        if(addEyeVisionWaiting || clearAreaWaiting){
            return;
        }
        if(visionInstances == null){
            visionInstances = new Dictionary<GameObject, float>();
        }
        currentVisionInstance = Instantiate(visionPrefabe);
        fogArea.ApendUnit(currentVisionInstance.GetComponent<IVision>());
        addEyeVisionWaiting = true;
    }

    private bool clearAreaWaiting = false;
    int currentClearRequestID = -1;
    public void ClearArea(){
        if(addEyeVisionWaiting || clearAreaWaiting){
            return;
        }
        if(fogClearRequests == null){
            fogClearRequests = new Dictionary<int, float>();
        }
        getSelectionSphere.SetActive(true);
        Vector3 pos = selectPosition();
        int clearRequest = -1;
        fogArea.ClearArea(pos,10,out clearRequest);
        currentClearRequestID = clearRequest;
        clearAreaWaiting = true;
    }

    Dictionary<int,float> fogClearRequests;
    Vector3 selectPosition(){
        

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycast = new RaycastHit();
        if(Physics.Raycast(Camera.main.transform.position,ray.direction,out raycast,1000)){
            getSelectionSphere.transform.position = raycast.point;
        }
        return raycast.point;
    }
    bool selectPosition(GameObject target,out Vector3 pos){
        
        pos = Vector3.zero;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var raycasts = Physics.RaycastAll(Camera.main.transform.position,ray.direction,1000);
        foreach(var raycast in raycasts){
            if(raycast.collider.gameObject == target){
                getSelectionSphere.transform.position = raycast.point;
                pos = raycast.point;
                return true;
            }else{
                return false;
            }
        }

        return false;
    }
    public GameObject plane;
    void Update(){
        if(clearAreaWaiting){
            var pos = selectPosition();
            if(currentClearRequestID != -1){
                fogArea.MotifyClearArea(currentClearRequestID,pos);
            }
            if(Input.GetKeyDown(KeyCode.Mouse0)){
                fogClearRequests.Add(currentClearRequestID,Time.time);
                currentClearRequestID = -1;
                clearAreaWaiting = false;
                getSelectionSphere.SetActive(false);

            }
        }
        if(fogClearRequests != null){
            List<int> keys = new List<int>();
            foreach(var key in fogClearRequests.Keys){
                keys.Add(key);
            }
            foreach(var key in keys){
                if(Time.time - fogClearRequests[key] > 5.0f){
                    fogArea.RemoveAreaClearRequest(key);
                    fogClearRequests.Remove(key);
                }
            }
        }
        
        if(addEyeVisionWaiting){
            var pos = Vector3.zero;
            if(currentVisionInstance != null && selectPosition(plane,out pos)){
                currentVisionInstance.transform.position = pos;
            }
            if(Input.GetKeyDown(KeyCode.Mouse0)){
                visionInstances.Add(currentVisionInstance,Time.time);
                currentVisionInstance = null;
                addEyeVisionWaiting = false;
            }
        }
        if(visionInstances != null){
            List<GameObject> keys = new List<GameObject>();
            foreach(var key in visionInstances.Keys){
                keys.Add(key);
            }
            foreach(var key in keys){
                if(Time.time - visionInstances[key] > 5.0f){
                    fogArea.RemoveUnit(key.GetComponent<IVision>());
                    visionInstances.Remove(key);
                    DestroyImmediate(key);
                }
            }
        }
    }
}
