using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace ThirdPart{
	[CustomEditor(typeof(FogArea))]
	public class FOWEditpr : Editor {
		FogArea selected;
		private bool drawBasic = true;
		private bool externalTool;
		private bool visualize;
		#region  Minimap Conf
		private static LayerMask miniMapLayermask = ~0;
		private static CameraClearFlags clearFlags = CameraClearFlags.SolidColor;
		private static Color bgColor = new Color(0.5f,0.8f,1f,1f);
		private static Shader replacementShader;
		private static float gamma = 0.45f;
		private static float saturate = 1.0f;
		private static float brightness = 1.0f;
		private static string replacementTag;
		private static FogArea.Resolution resolution = FogArea.Resolution.R256;
		#endregion
		
		// Use this for initialization
		void OnEnable () {
			selected = target as FogArea;
			miniMapLayermask = ~0;//<< LayerMask.NameToLayer("FoWMask");
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			if(Camera.main != null){
				if(Camera.main.depthTextureMode == DepthTextureMode.None){
					EditorGUILayout.HelpBox("Current scene MainCamera does not enable Depth, The fog renderer maybe rendered incorrect (Fog shader need CameraDepthTexture to calculate world space position)",MessageType.Warning);
					if(GUILayout.Button("Quick Fix")){
						Undo.RecordObject(Camera.main,"ChangeDepthModel");
						Camera.main.depthTextureMode = DepthTextureMode.Depth;
						EditorUtility.SetDirty(Camera.main);
						Debug.Log("MainCamera depth texture mode change to DepthTextureMode.Depth");
					}
				}
			}
#if UNITY_2019OR_NEWER
            drawBasic = EditorGUILayout.BeginFoldoutHeaderGroup(drawBasic,"Basic");
#else
            drawBasic = EditorGUILayout.Foldout( drawBasic, "Basic");

#endif
            if (drawBasic){
                OnDrawBasic();
            }
#if UNITY_2019OR_NEWER
			EditorGUILayout.EndFoldoutHeaderGroup();
#endif

#if UNITY_2019OR_NEWER
            externalTool = EditorGUILayout.BeginFoldoutHeaderGroup(externalTool,"External Tool");
#else
            externalTool = EditorGUILayout.Foldout(externalTool, "External Tool");
#endif
            if (externalTool){
				OnDrawExternalTool();
			}
#if UNITY_2019OR_NEWER
			EditorGUILayout.EndFoldoutHeaderGroup();
#endif

#if UNITY_2019OR_NEWER
            visualize = EditorGUILayout.BeginFoldoutHeaderGroup(visualize,"Visualize");
#else
            visualize = EditorGUILayout.Foldout(visualize, "Visualize");
#endif
            if (visualize){
				OnVisualize();
			}
#if UNITY_2019OR_NEWER
			EditorGUILayout.EndFoldoutHeaderGroup();
#endif

            serializedObject.ApplyModifiedProperties();

		}

		void OnDrawBasic(){
			SerializedProperty areaSize = serializedObject.FindProperty("areaSize");
			EditorGUILayout.PropertyField(areaSize);
			SerializedProperty resoltion = serializedObject.FindProperty("resolution");
			EditorGUILayout.PropertyField(resoltion);
			SerializedProperty ObstacleMapUpdate = serializedObject.FindProperty("updateMode");
			EditorGUILayout.PropertyField(ObstacleMapUpdate);
			if(selected.updateMode == FogArea.ObstacleMapUpdateType.Custom){
				SerializedProperty customObstacleMap = serializedObject.FindProperty("customObstacleMap");
				EditorGUILayout.PropertyField(customObstacleMap);
			}
			SerializedProperty ObstacleLayer = serializedObject.FindProperty("ObstacleLayer");
			EditorGUILayout.PropertyField(ObstacleLayer);
			SerializedProperty smoothFade = serializedObject.FindProperty("smoothFade");
			EditorGUILayout.Slider(smoothFade,0.02f,1f);
			SerializedProperty fogAreaOpacity = serializedObject.FindProperty("fogAreaOpacity");
			EditorGUILayout.Slider(fogAreaOpacity,0.1f,1f);
			SerializedProperty blurryFogMap = serializedObject.FindProperty("blurryFogMap");
			EditorGUILayout.Slider(blurryFogMap,0.0f,2f);
			SerializedProperty mapRefreshRate = serializedObject.FindProperty("mapRefreshRate");
			EditorGUILayout.Slider(mapRefreshRate,10,60);
			SerializedProperty upScale = serializedObject.FindProperty("upScale");
			EditorGUILayout.PropertyField(upScale);
			SerializedProperty OutputTarget = serializedObject.FindProperty("OutputTarget");
			EditorGUILayout.PropertyField(OutputTarget);
			SerializedProperty fogRenderer = serializedObject.FindProperty("renderer");
			EditorGUILayout.PropertyField(fogRenderer);
			string btnName = "Generate Renederer";
			if(fogRenderer.objectReferenceValue == null){
				btnName = "Generate Renederer";
			}else{
				btnName = "Update Renederer";
			}
			if(GUILayout.Button(btnName)){
				selected.GenerateRenderer();
				serializedObject.ApplyModifiedProperties();
			}
			if (GUILayout.Button ("Generate Obstacle Map")) {
				selected.GenerateObstacleMap();
			}

		}

		void OnDrawExternalTool(){
			EditorGUILayout.HelpBox("You can use any custom Shader as replacement shader to rendering minimap",MessageType.Info);
			replacementShader = EditorGUILayout.ObjectField("Replacement",replacementShader,typeof(Shader),false) as Shader;
			var tempMask = EditorGUILayout.MaskField("Render Layer", InternalEditorUtility.LayerMaskToConcatenatedLayersMask(miniMapLayermask), InternalEditorUtility.layers);
			miniMapLayermask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
			clearFlags = (CameraClearFlags)EditorGUILayout.EnumPopup("Clear Flag",clearFlags);
			resolution = (FogArea.Resolution)EditorGUILayout.EnumPopup("Resolution",resolution);
			bgColor = EditorGUILayout.ColorField("Background Color",bgColor);
			gamma = EditorGUILayout.Slider("Gamma",gamma,0,2);
			saturate = EditorGUILayout.Slider("Saturate",saturate,0,2);
			brightness = EditorGUILayout.Slider("Brightness",brightness,0,2);
			if (GUILayout.Button ("Render Mini Map")) {
				GameObject buf = new GameObject ("_miniCam_Buf");
				Camera obCam = buf.AddComponent<Camera> ();
				buf.SetActive(false);
				obCam.enabled = false;
				obCam.transform.SetParent (selected.transform);
				obCam.transform.localPosition = Vector3.up * 10;
				obCam.transform.rotation = Quaternion.LookRotation (selected.transform.up * -1, selected.transform.forward);
				obCam.orthographic = true;
				obCam.orthographicSize = selected.areaSize * 0.5f;
				obCam.nearClipPlane = 0;
				obCam.farClipPlane = 20;
				obCam.clearFlags = clearFlags;
				obCam.cullingMask = miniMapLayermask;
				obCam.backgroundColor = bgColor;
				obCam.renderingPath = RenderingPath.Forward;
				RenderTexture RT = new RenderTexture((int)resolution,(int)resolution,0,RenderTextureFormat.ARGB32);
				RT.Create();
				if(replacementShader != null){
					obCam.SetReplacementShader(replacementShader,replacementTag);
				}
				int needRestore = -1;
				if(selected.renderer != null){
					needRestore = selected.renderer.gameObject.activeSelf ? 1 : 0;
					selected.renderer.gameObject.SetActive(false);
				}
				obCam.targetTexture = RT;
				obCam.Render();
				obCam.targetTexture = null;
				if(needRestore == 1){
					selected.renderer.gameObject.SetActive(true);
				}
				Texture2D export = new Texture2D ((int)resolution, (int)resolution, TextureFormat.RGBA32,false);
				var colors = RT.ReadColors(gamma,saturate,brightness);
				export.SetPixels (colors);
				export.Apply ();
				string path = EditorUtility.SaveFilePanelInProject ("Save to file", "NewMiniMap", "png", "Save");
				if (path != "") {
					byte[] img = export.EncodeToPNG ();
					System.IO.File.WriteAllBytes (path, img);
					AssetDatabase.Refresh ();
				}
				DestroyImmediate(buf);
				RT.Release();
				DestroyImmediate(RT);
			}
			if (GUILayout.Button ("Export Obstacle Map")) {
				var tex = selected.ExportObstacleMap();
				string path = EditorUtility.SaveFilePanelInProject ("Save to file", "NewObstacleMap", "png", "Save");
				if (path != "") {
					byte[] img = tex.EncodeToPNG ();
					System.IO.File.WriteAllBytes (path, img);
					AssetDatabase.Refresh ();
				}
			}
		}

		void OnVisualize(){
			
			if (selected.ObstacleMap) {
				GUILayout.Label("left: Obstacle map , right: vision map");
				GUILayout.Space(10);
				Vector2 pos = GUILayoutUtility.GetLastRect ().position + (GUILayoutUtility.GetLastRect ().size.y) * Vector2.up;
				float w = GUILayoutUtility.GetAspectRect (1).size.x * 0.5f;
				Rect rect =new Rect (pos, new Vector2(w,w));
				EditorGUILayout.BeginHorizontal();
				GUI.DrawTexture (rect, selected.ObstacleMap);
				rect =new Rect (pos + new Vector2(w + 2,0), new Vector2(w,w));
				GUI.DrawTexture (rect, selected.GetFogTexture);
				EditorGUILayout.EndHorizontal();
			}
			
			Repaint();

		}
	}
}