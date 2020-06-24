using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class JoyStickGraphic : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    private static Dictionary<string, JoyStickGraphic> JoystickValuePool;
    public static bool TryGetAxis(string joystickTag,out Vector2 value) {
        value = Vector2.zero;
        if (JoystickValuePool == null) {
            JoystickValuePool = new Dictionary<string, JoyStickGraphic>();
            return false;
        } else {
            if (JoystickValuePool.ContainsKey(joystickTag)) {
                value = JoystickValuePool[joystickTag].getAxis;
                return true;
            } else {
                return false;
            }
        }
        return false;
    }
    [SerializeField]
    private string JoyStickTag;
    [SerializeField]
    private RectTransform handle;
    private RectTransform thisRect;
    [Header("use native input")]
    public string syncWithAxisInput_X = "Horizontal";
    public string syncWithAxisInput_Y = "Vertical";
    void Start()
    {
        thisRect = GetComponent<RectTransform>();
        var graphic = handle.GetComponent<Graphic>();
        if(graphic != null) {
            graphic.raycastTarget = false;
        }
    }
    void OnEnable() {
        if(JoystickValuePool == null) {
            JoystickValuePool = new Dictionary<string, JoyStickGraphic>();
        }
        if(JoyStickTag != "") {
            if (JoystickValuePool.ContainsKey(JoyStickTag)) {
                Debug.LogError("Joystick name already exist , be careful !");
            } else {
                JoystickValuePool.Add(JoyStickTag, this);
            }
        }
    }
    void OnDisable() {
        if (JoystickValuePool == null) {
            return;
        }
        if (JoyStickTag != "") {
            if (JoystickValuePool.ContainsKey(JoyStickTag)) {
                JoystickValuePool.Remove(JoyStickTag);
            }
        }
    }
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
        usNativeAxisInput = false;
    }
    public Vector2 axis;
    public Vector2 getAxis {
        get {
            return axis;
        }
    }
    private bool usNativeAxisInput;
    void IDragHandler.OnDrag(PointerEventData eventData) {
        Vector2 pointer = eventData.position;
        float sx = (float)Screen.currentResolution.width / (float)Display.main.systemWidth;
        float sy = (float)Screen.currentResolution.height / (float)Display.main.systemHeight;
        pointer = pointer.mul(new Vector2(1.0f / sx, 1.0f / sy));
        axis = pointer- thisRect.anchoredPosition;
        float dis = thisRect.sizeDelta.magnitude * 0.3f;
        float clampedDis = Mathf.Clamp(axis.magnitude, 0, dis) / dis;
        axis = axis.normalized * clampedDis;
        handle.anchoredPosition = axis * thisRect.sizeDelta.magnitude * 0.3f;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
        axis = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
    void LateUpdate() {
        if(syncWithAxisInput_X != "" && syncWithAxisInput_Y != "") {
            float ax  = Input.GetAxis(syncWithAxisInput_X);
            float ay = Input.GetAxis(syncWithAxisInput_Y);
            if(ax != 0 || ay != 0) {
                usNativeAxisInput = true;
            }
            if (usNativeAxisInput) {
                axis = new Vector2(ax, ay);
                axis = axis.normalized * Mathf.Clamp01(axis.magnitude);
                handle.anchoredPosition = axis * thisRect.sizeDelta.magnitude * 0.3f;
            }
        }
    }
    
}
public static class ExternalV2 {
    public static Vector2 mul(this Vector2 v0, Vector2 v1) {
        return new Vector2(v0.x * v1.x, v0.y * v1.y);
    }
}