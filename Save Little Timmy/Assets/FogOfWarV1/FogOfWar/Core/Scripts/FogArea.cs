using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;



namespace ThirdPart{
    [DisallowMultipleComponent]
	[ExecuteInEditMode]
	public class FogArea : MonoBehaviour {
		#region  Define
		private const string obstacleShader = "Hidden/ObstacleRender";
		private const string fogOfWarShader = "Hidden/FogOfWar";
        private const float minVisiableFogDensity = 0.1f;
		public delegate void FOWEventHandler();
		public enum Resolution
		{
			R64 = 64, R128 = 128, R256 = 256,R512 = 512
		}
		private enum RenderPass
		{
			RenderVision = 0,
			Clear = 1,
			Fade = 2,
			Blur = 3,
			FormatToAlphaTex = 4,
		}

		private enum FunctionCSKernel
		{
			Clear = 0, RenderDynamicOccluder =1, ExposeData = 2
		}
		public enum ObstacleMapUpdateType{
			OnAwake,EveryFrame,Custom
		}
		/// <summary>
        /// Area clear request
        /// </summary>
		private struct FoWClearRequest{
            /// <summary>
            /// clear area world position
            /// </summary>
			public Vector3 position;
            /// <summary>
            /// clear area world radius
            /// </summary>
			public float radius;
            /// <summary>
            /// controll ID
            /// </summary>
			public int controllID{
				get;
				private set;
			}
			public FoWClearRequest(Vector3 position,float radius,int id){
				this.controllID = id;
				this.position = position;
				this.radius = radius;
			}
			public Vector3 ToGPUData(FogArea fogArea){
				var pos = FOWExternal.WorldToFogAreaCoord(position,fogArea);
				float toTexRadius = radius / fogArea.areaSize;
				return new Vector3(pos.x,pos.y,toTexRadius);
			}

		}
		#endregion

		#region Params
		[Tooltip("Fog mask area size")]
		public float areaSize = 100;
		[Tooltip("Fog area texture resolution,Higer is more detail but more GPU usage")]
		public Resolution resolution = Resolution.R128;
		[Tooltip("If you want to use custom Obstacle map, please use [Custom] model")]
		public ObstacleMapUpdateType updateMode;
		[Tooltip("Fog Obstacle layer (for render ObstacleMap)")]
		public LayerMask ObstacleLayer;
		[Tooltip("speed of fog smooth fade")]
		[Range(0.02f,1f)]
		public float smoothFade = 0.5f;
		[Tooltip("Tranceparence of the fog,*motify material color manually if you want change the fog color ")]
		[Range(0.1f,1f)]
		public float fogAreaOpacity= 0.8f;
		[Tooltip("Controll fog edge cross fade smooth or hard")]
		[Range(0,2)]
		public float blurryFogMap = 1.0f;
		[Range(10,60)]
		[Tooltip("Map refresh times in one sec ,Lower is better performance")]
		public float mapRefreshRate = 20;

		[Tooltip("Calculation always done in target resolution ,but enable upScale can get better result")]
		public bool upScale = true;
		public int UpScale {
			get{
				return upScale ? 2 : 1;
			}
		}
		[Tooltip("If set, this component will automatically Copy Fog Texture to target (.RGB = 1,.A = FogTexture.R) on every update frame,useful for mini-map ui")]
		public RenderTexture OutputTarget;
		public int texelSize{
			get{ 
				return (int)resolution;
			}
		}
		[Tooltip("Just need Red Chanel (Single Chanel), 1 =  Obstacled , 0 = Flat area , useful for irregular height terrain (require more Art works)")]
		public Texture customObstacleMap;
		
		[Tooltip("The fog area renderer, it can be auto generate(click the button under this field)")]
		[SerializeField]
		public Renderer renderer;
		#endregion
		
		private ComputeShader fowFuncCS;
		private RenderTexture fogTexture;
		public RenderTexture GetFogTexture{
			get{
				return fogTexture;
			}
		}
		private RenderTexture smothFadedFogTexture_Buffer;
		private RenderTexture smothFadedFogTexture;
		private RenderTexture blurredFogTexture;
		private RenderTexture dataExchange;
		private RenderTexture obastacleMap;
		private RenderTexture dynamicObstacle;
		public RenderTexture ObstacleMap{
			get{ 
				return obastacleMap;
			}
		}
		// private RenderTexture upscaledSmothFogTexture;
		private Shader obstacleRender;
		private Material fogOfWarMat;
		private List<IVision> visionUnit;
		private System.Random random;
		public event FOWEventHandler OnFogMapUpdated = delegate {};


		private float lastRefreshTime;
		private float refreshStep{
			get{ 
				return 1.0f / mapRefreshRate;
			}
		}

		// Use this for initialization


		void OnEnable () {
			Inti_MainData ();
			InitUnitData ();
			OnRenderObstacleMap ();
			InitRenderTextures ();
			Init_DynamicOccludeData ();
			OnRenderDynamicObstacle ();
			RenderFOWVision();
			Debug.Log("Fog Area Initialized");
			
		}

		void OnDisable(){
			Graphics.ClearRandomWriteTargets ();
			RenderTexture.active = null;
			if (fogTexture) {
				fogTexture.DiscardContents ();
				DestroyImmediate (fogTexture);
			}
			if (obastacleMap) {
				obastacleMap.DiscardContents ();
				DestroyImmediate (obastacleMap);
			}
			if (dataExchange) {
				dataExchange.DiscardContents ();
				DestroyImmediate (dataExchange);
			}
			if (blurredFogTexture) {
				blurredFogTexture.DiscardContents ();
				DestroyImmediate (blurredFogTexture);
			}
			if (smothFadedFogTexture) {
				smothFadedFogTexture.DiscardContents ();
				DestroyImmediate (smothFadedFogTexture);
			}
			if (smothFadedFogTexture_Buffer) {
				smothFadedFogTexture_Buffer.DiscardContents ();
				DestroyImmediate (smothFadedFogTexture_Buffer);
			}
			if (dOccluderBuffer != null) {
				dOccluderBuffer.Dispose();
				dOccluderBuffer = null;
			}
			if (fogInfo != null) {
				fogInfo.Dispose ();
				fogInfo = null;
			}
		}
		void OnDestroy(){
			OnFogMapUpdated = delegate {};
		}
		void Inti_MainData(){
			fowFuncCS = Resources.Load<ComputeShader> ("FOWFuntionCS");
			if (fogOfWarMat == null) {
				fogOfWarMat = new Material (Shader.Find(fogOfWarShader)); 
			}
			random = new System.Random(System.DateTime.Now.Second + System.DateTime.Now.Millisecond);

		}

		#region  ---------------------------------------------------Obstacle Map DATA-----------------------------------------------------------
		private ComputeBuffer fogInfo;
		private float[] pixels;
		private int dataLength {
			get{
				return texelSize * texelSize;
			}
		}
		private const int TYPE_Pixel_Byte_Length = 4; // 1 * UpScale byte
		void InitUnitData(){
			fogInfo = new ComputeBuffer (dataLength,TYPE_Pixel_Byte_Length);
			pixels = new float[dataLength];
			fogInfo.SetData (pixels);
			fogOfWarMat.SetBuffer ("_FogPixelBuffer", fogInfo);

			visionUnit = new List<IVision> ();
			maskableUnit = new List<IMaskableUnit> ();
			visionUnit = FOWExternal.FindTargetsOfType<IVision> ();
			maskableUnit = FOWExternal.FindTargetsOfType<IMaskableUnit> ();
			foreach (var unit in maskableUnit) {
				unit.SetFogArea (this);
			}
		}
		#endregion
		#region ---------------------------------------------------- Interface -----------------------------------------------------
		private List<IMaskableUnit> maskableUnit;
		public const int FOW_EXCEPTION_NULLREFERENCE = 0;
		public const int FOW_EXCEPTION_OUTOFAREA = 1;
		/// <summary>
		/// Get Buffer Index form UV coord
		/// </summary>
		/// <param name="uvX">UV.x</param>
		/// <param name="uvY">UV.y</param>
		/// <returns></returns>
		public int UVToIndex(int uvX,int uvY){
			return uvY * texelSize + uvX;
		}
		/// <summary>
		/// Get FogArea texture alpha value (at the coord of the unit position)
		/// </summary>
		/// <param name="unit"></param>
		/// <returns></returns>
		public float GetBufferValue(IMaskableUnit unit){
			int uvX = 0;
			int uvY = 0;
			unit.MappingToUV (this, out uvX, out uvY);
			int index = UVToIndex(uvX,uvY);
			return pixels [index];
		}
        /// <summary>
        /// Get FogArea texture alpha value from world position
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public float GetBufferValue(Vector3 worldPos) {
            int uvX = 0;
            int uvY = 0;
            FOWExternal.MappingToUV(worldPos,this, out uvX, out uvY);
            int index = UVToIndex(uvX, uvY);
            return pixels[index];
        }

        #region  -------------------------Use those method to add/remove unit dynamicly------------------------
        /// <summary>
        /// Apend New Maskable Unit
        /// </summary>
        /// <param name="unit"></param>
        public void ApendUnit(IMaskableUnit unit){
			if(maskableUnit.Contains(unit)){
				Debug.LogError("Target unit already exist in unit pool");
				return;
			}else{
				maskableUnit.Add(unit);
				unit.SetFogArea(this);
			}
		}
		/// <summary>
		/// Remove maskable unit
		/// </summary>
		/// <param name="unit"></param>
		public void RemoveUnit(IMaskableUnit unit){
			if(maskableUnit.Contains(unit)){
				maskableUnit.Remove(unit);
			}else{
				Debug.LogError("Target unit not exist in unit pool");
				return;
			}
		}
		/// <summary>
		/// Apend New Vision Unit
		/// </summary>
		/// <param name="unit"></param>
		public void ApendUnit(IVision unit){
			if(visionUnit.Contains(unit)){
				Debug.LogError("Target unit already exist in unit pool");
				return;
			}else{
				visionUnit.Add(unit);
			}
		}
		/// <summary>
		/// Remove vision unit
		/// </summary>
		/// <param name="unit"></param>
		public void RemoveUnit(IVision unit){
			if(visionUnit.Contains(unit)){
				visionUnit.Remove(unit);
			}else{
				Debug.LogError("Target unit not exist in unit pool");
				return;
			}
		}
		#endregion
		/// <summary>
		/// is target visible in the fog area
		/// </summary>
		/// <param name="unit">unit</param>
		/// <param name="error">error code</param>
		/// <returns></returns>
		public bool TargetIsVisible(IMaskableUnit unit,out int error){
			error = -1;
			if (maskableUnit.Contains (unit)) {
				int uvX = 0;
				int uvY = 0;
				unit.MappingToUV (this, out uvX, out uvY);
				if (uvX < 0 || uvX > texelSize - 1 || uvY < 0 || uvY > texelSize - 1) {
					error = FOW_EXCEPTION_OUTOFAREA;
					return false;
				}
				int index = UVToIndex(uvX,uvY);

				if(index < 0 || index >= pixels.Length){ // out of range
					return false;
				}
				try{
					return pixels [index] > minVisiableFogDensity;
				}catch(Exception ex){
					Debug.LogError ("Wrong Index: " + index);
					throw ex;
				}
			} else {
				error = FOW_EXCEPTION_NULLREFERENCE;
				return false;
			}
		}
        /// <summary>
        /// is target position visible in the fog area
        /// </summary>
        /// <param name="worldPos"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool TargetIsVisible(Vector3 worldPos, out int error) {
            error = -1;
            int uvX = 0;
            int uvY = 0;
            FOWExternal.MappingToUV(worldPos, this, out uvX, out uvY);
            if (uvX < 0 || uvX > texelSize - 1 || uvY < 0 || uvY > texelSize - 1) {
                error = FOW_EXCEPTION_OUTOFAREA;
                return false;
            }
            int index = UVToIndex(uvX, uvY);

            if (index < 0 || index >= pixels.Length) { // out of range
                return false;
            }
            try {
                return pixels[index] > minVisiableFogDensity;
            } catch (Exception ex) {
                Debug.LogError("Wrong Index: " + index);
                throw ex;
            }
        }

        private Dictionary<int,FoWClearRequest> clearAreaRequests;
		/// <summary>
		/// Clear the fog of target area (Include Obstacle area)
		/// </summary>
		/// <param name="worldPos">target position in the world</param>
		/// <param name="worldRadius">clear radius in the world(m)</param>
		/// <param name="controllID">controll ID</param>
		public void ClearArea(Vector3 worldPos,float worldRadius,out int controllID){
			if(clearAreaRequests == null){
				clearAreaRequests = new Dictionary<int, FoWClearRequest>();
			}
			do{
				if(random == null){
					random = new System.Random(System.DateTime.Now.Second + System.DateTime.Now.Millisecond);
				}
				controllID = random.Next(10000,99999);
			}while(clearAreaRequests.ContainsKey(controllID));
			FoWClearRequest request = new FoWClearRequest(worldPos,worldRadius,controllID);
			clearAreaRequests.Add(controllID,request);
		}
		/// <summary>
		/// Remove clear area request
		/// </summary>
		/// <param name="controllID"></param>
		public void RemoveAreaClearRequest(int controllID){
			if(clearAreaRequests == null){
				clearAreaRequests = new Dictionary<int, FoWClearRequest>();
				return;
			}
			if(clearAreaRequests.ContainsKey(controllID)){
				clearAreaRequests.Remove(controllID);
			}
		}
		#region Clear Request Motify
		public void MotifyClearArea(int controllID,Vector3 newWorldPosition){
			if(clearAreaRequests == null){
				clearAreaRequests = new Dictionary<int, FoWClearRequest>();
				return;
			}
			if(!clearAreaRequests.ContainsKey(controllID)){
				return;
			}
			var temp = clearAreaRequests[controllID];
			temp.position = newWorldPosition;
			clearAreaRequests[controllID] = temp;
		}
		public void MotifyClearArea(int controllID,float newRadius){
			if(clearAreaRequests == null){
				clearAreaRequests = new Dictionary<int, FoWClearRequest>();
				return;
			}
			var temp = clearAreaRequests[controllID];
			temp.radius = newRadius;
			clearAreaRequests[controllID] = temp;
		}
		public void MotifyClearArea(int controllID,Vector3 newWorldPosition,float newRadius){
			if(clearAreaRequests == null){
				clearAreaRequests = new Dictionary<int, FoWClearRequest>();
				return;
			}
			var temp = clearAreaRequests[controllID];
			temp.position = newWorldPosition;
			temp.radius = newRadius;
			clearAreaRequests[controllID] = temp;
		}


		#endregion
		/// <summary>
		/// Auto Create Fog Renderer
		/// </summary>
		public void GenerateRenderer(){
			if(renderer != null){
				DestroyImmediate(renderer.gameObject);
			}
			GameObject renererGO = new GameObject("FogAreaRenderer");
			renererGO.transform.SetParent(this.transform);
			renererGO.transform.localPosition = Vector3.zero;
			var fogRenderer = renererGO.AddComponent<FogAreaRenderer>();
			((IFogRenderer)fogRenderer).InitializeRenderer(this);
			renderer = fogRenderer.renderer;
		}
		/// <summary>
		/// Copy ObstacleMap to a Texture
		/// </summary>
		/// <returns></returns>
		public Texture2D ExportObstacleMap(){
			Texture2D export = new Texture2D (texelSize, texelSize, TextureFormat.RGBA32,false);
			Color[] colors = obastacleMap.ReadColors ();
			export.SetPixels (colors);
			export.Apply ();
			return export;
		}
		/// <summary>
		/// Generate ObstacleMap 
		/// </summary>
		[ContextMenu("GenerateObstacleMap")]
		public void GenerateObstacleMap(){
			float hightestBoundsTop = 10f;
			Renderer[] renderers = GameObject.FindObjectsOfType<Renderer>();
			foreach(var item in renderers){
				int objLayerMask = (1 << item.gameObject.layer); 
				if((ObstacleLayer.value & objLayerMask) > 0){
					hightestBoundsTop = Mathf.Max(hightestBoundsTop, item.bounds.max.y * 0.5f + item.transform.position.y + 0.1f);
				}
			}
			ObstacleRenderCamViewLength = hightestBoundsTop;
			OnRenderObstacleMap ();
		}
		#endregion

		#region  ---------------------------------------Internal Logic------------------------------
		void OnRenderObstacleMap(){
			if(customObstacleMap == null){
				GameObject buf = new GameObject ("_obCam_Buf");
				Camera obCam = buf.AddComponent<Camera> ();
				// buf.hideFlags = HideFlags.HideAndDontSave;
				// obCam.hideFlags = HideFlags.HideAndDontSave;
				buf.SetActive(false);
				obCam.enabled = false;
				obCam.transform.SetParent (this.transform);
				obCam.transform.localPosition = Vector3.up * ObstacleRenderCamViewLength;
				obCam.transform.rotation = Quaternion.LookRotation (this.transform.up * -1, this.transform.forward);
				obCam.orthographic = true;
				obCam.orthographicSize = areaSize * 0.5f;
				obCam.nearClipPlane = 0;
				obCam.farClipPlane = ObstacleRenderCamViewLength - obstacleRenderBaise;
				obCam.clearFlags = CameraClearFlags.Color;
				obCam.cullingMask = ObstacleLayer;
				obCam.backgroundColor = new Color (0, 0, 0, 0);
				obCam.renderingPath = RenderingPath.Forward;
				obCam.depthTextureMode = DepthTextureMode.Depth;
				
				if (obstacleRender == null) {
					obstacleRender = Shader.Find (obstacleShader);
				}
				if(obastacleMap == null){
					obastacleMap = new RenderTexture (texelSize,texelSize,0,RenderTextureFormat.RFloat);
					obastacleMap.filterMode = FilterMode.Bilinear;
					obastacleMap.useMipMap = true;
					obastacleMap.autoGenerateMips = true;
				}
				obCam.targetTexture = obastacleMap;
				obCam.RenderWithShader (obstacleRender,"RenderType");
				obCam.targetTexture = null;
				DestroyImmediate (obCam.gameObject);
			}
			fogOfWarMat.SetTexture ("_ObstacleMap", updateMode == ObstacleMapUpdateType.Custom? customObstacleMap : obastacleMap);
			
		}

		void CheckSettings(){
			if (fogTexture == null) {
				OnEnable ();
				Debug.Log("FogTexture Initialized");
				return;
			}
			if ((int)resolution != fogTexture.width) {
				OnDisable ();
				OnEnable ();
				Debug.LogWarning("Resolution Changed");
			}
		}
		
		void InitRenderTextures(){
			#region  Check
			if (fogTexture) {
				fogTexture.DiscardContents ();
				DestroyImmediate (fogTexture);
			}
			if (dataExchange) {
				dataExchange.DiscardContents ();
				DestroyImmediate (dataExchange);
			}
			if (blurredFogTexture) {
				blurredFogTexture.DiscardContents ();
				DestroyImmediate (blurredFogTexture);
			}
			if (smothFadedFogTexture) {
				smothFadedFogTexture.DiscardContents ();
				DestroyImmediate (smothFadedFogTexture);
			}
			if (smothFadedFogTexture_Buffer) {
				smothFadedFogTexture_Buffer.DiscardContents ();
				DestroyImmediate (smothFadedFogTexture_Buffer);
			}
			#endregion
			if(obastacleMap == null){
				obastacleMap = new RenderTexture (texelSize,texelSize,0,RenderTextureFormat.RFloat);
				obastacleMap.filterMode = FilterMode.Bilinear;
				obastacleMap.useMipMap = true;
				obastacleMap.autoGenerateMips = true;
			}
			dynamicObstacle = new RenderTexture (obastacleMap);
			fogTexture = new RenderTexture (obastacleMap);
			dataExchange = new RenderTexture (obastacleMap);
			blurredFogTexture = new RenderTexture(texelSize * UpScale,texelSize * UpScale,0,RenderTextureFormat.RHalf);
			smothFadedFogTexture = new RenderTexture(texelSize * UpScale,texelSize * UpScale,0,RenderTextureFormat.RHalf);
			smothFadedFogTexture_Buffer = new RenderTexture(texelSize * UpScale,texelSize * UpScale,0,RenderTextureFormat.RHalf);

		}
		
		void RenderFOWVision(){
			
			List<Vector4> visionBufferData = new List<Vector4>();
			if(visionUnit == null){
				InitUnitData(); 
			}
			if(visionUnit.Count == 0){
				return;
			}
			//Unit Vision Data
			for (int i = 0; i < visionUnit.Count; i++) {
				Vector4 parm = new Vector4 ();
				Vector4 last = new Vector4();
				try{
					if (visionUnit [i].NeedCalculate (this, out parm,out last)) {
						visionBufferData.Add(parm);
					}
				}catch(Exception ex){
					Debug.Log(ex.Message);
					visionUnit.RemoveAt(i);
				}
			}
			ComputeBuffer visionBuffer= new ComputeBuffer(visionBufferData.Count,16);
			ComputeBuffer clearRequestBuffer = null;
			visionBuffer.SetData(visionBufferData);
			fogOfWarMat.SetBuffer("_VisionData",visionBuffer);
			fogOfWarMat.SetInt("_VisionCount",visionBufferData.Count);

			//Clear Area Data
			if(clearAreaRequests != null){
				List<Vector3> clearAreaBufferData = new List<Vector3>();
				foreach(var item in clearAreaRequests){
					clearAreaBufferData.Add(item.Value.ToGPUData(this));
				}
				if(clearAreaBufferData.Count > 0){
					clearRequestBuffer= new ComputeBuffer(clearAreaBufferData.Count,12);
					clearRequestBuffer.SetData(clearAreaBufferData.ToArray());
					fogOfWarMat.SetBuffer("_ClearAreaRequest",clearRequestBuffer);
				}

				fogOfWarMat.SetInt("_ClearAreaRequestCount",clearAreaBufferData.Count);
			}else{
				fogOfWarMat.SetInt("_ClearAreaRequestCount",0);
			}

			Graphics.Blit (null, fogTexture, fogOfWarMat, (int)RenderPass.RenderVision);
			fogOfWarMat.SetTexture ("_OldFogTexture", smothFadedFogTexture_Buffer);
			fogOfWarMat.SetFloat ("_fadeSpeed", smoothFade);
			fogOfWarMat.SetInt("_sourceFogMapPixelSize",texelSize);
			Graphics.Blit (fogTexture, smothFadedFogTexture,fogOfWarMat,(int)RenderPass.Fade);
			Graphics.Blit (smothFadedFogTexture, smothFadedFogTexture_Buffer);
			fogOfWarMat.SetFloat("_Blurry",blurryFogMap);
			fogOfWarMat.SetVector ("_Kernal", new Vector4 (1, 0, 0, 0));
			Graphics.Blit (smothFadedFogTexture, blurredFogTexture,fogOfWarMat,(int)RenderPass.Blur);
			fogOfWarMat.SetVector ("_Kernal", new Vector4 (0, 1, 0, 0));
			Graphics.Blit (blurredFogTexture, smothFadedFogTexture,fogOfWarMat,(int)RenderPass.Blur);
			fogOfWarMat.SetVector ("_Kernal", new Vector4 (1.5f, 0, 0, 0));
			Graphics.Blit (smothFadedFogTexture, blurredFogTexture,fogOfWarMat,(int)RenderPass.Blur);
			fogOfWarMat.SetVector ("_Kernal", new Vector4 (0, 1.5f, 0, 0));
			Graphics.Blit (blurredFogTexture, smothFadedFogTexture,fogOfWarMat,(int)RenderPass.Blur);
			Graphics.Blit (smothFadedFogTexture, blurredFogTexture);
			Graphics.SetRandomWriteTarget (2, fogInfo);
			// Set Data
			fowFuncCS.SetInt("_source_width",texelSize);
			fowFuncCS.SetInt("_source_height",texelSize);
			fowFuncCS.SetTexture((int)FunctionCSKernel.ExposeData,"_bluredFogMap",blurredFogTexture);
			fowFuncCS.SetBuffer ((int)FunctionCSKernel.ExposeData, "_FogPixelBuffer", fogInfo);
			// Run
			fowFuncCS.Dispatch((int)FunctionCSKernel.ExposeData,texelSize/8,texelSize/8,1);
	
			Graphics.Blit(blurredFogTexture,OutputTarget,fogOfWarMat,(int)RenderPass.FormatToAlphaTex);

			Graphics.ClearRandomWriteTargets();

			visionBuffer.Dispose();
			if(clearRequestBuffer != null){
				clearRequestBuffer.Dispose();
			}

		}
		IEnumerator CopyDataAsync(){
			var request = AsyncGPUReadback.Request(fogInfo);
			yield return new WaitUntil(() => request.done);
			pixels = request.GetData<float>().ToArray();
		}
		public string errorInfo;
		void Update(){
			MaterialPropertyBlock matPptyBlock = new MaterialPropertyBlock();
			matPptyBlock.SetTexture ("_fogTexture", fogTexture);
			matPptyBlock.SetTexture ("_SmothedFogTexture", blurredFogTexture);
			matPptyBlock.SetFloat ("_FogAreaOpacity", fogAreaOpacity);
			matPptyBlock.SetMatrix ("_FogAreaWorldToLocal", transform.worldToLocalMatrix);
			matPptyBlock.SetVector ("_FogAreaProjectionParams", new Vector4(transform.position.x,transform.position.y,transform.position.z,areaSize * 0.5f));
			matPptyBlock.SetMatrix ("_FogAreaWorldToLocal", transform.worldToLocalMatrix);
			if(renderer != null){
				renderer.SetPropertyBlock(matPptyBlock);
			}else{
				Debug.LogError("Fog renderer is null");
			}
			if (Time.realtimeSinceStartup - lastRefreshTime > refreshStep) {
				CheckSettings ();
				if(updateMode == ObstacleMapUpdateType.EveryFrame){
					OnRenderObstacleMap();
				}
				OnRenderDynamicObstacle ();
				RenderFOWVision();
				OnFogMapUpdated.Invoke ();
				if(SystemInfo.supportsAsyncGPUReadback){
					errorInfo = "none";
					StartCoroutine(CopyDataAsync());
				}else{
					fogInfo.GetData(pixels);
					if(errorInfo == ""){
						Debug.LogError("This Device cant handle AsyncGPUReadback ! Performance will not be good");
					}
					errorInfo = "asyncReadbackFailed";
				}

				lastRefreshTime = Time.realtimeSinceStartup;
			}
		}
		#endregion

		#region ----------------------------------------DynamicObstacles------------------------------------------
		private ComputeBuffer dOccluderBuffer;
		private float[] occludeInfo;
		private List<IOccluder> dOccluders;
		void Init_DynamicOccludeData(){
			dOccluders = new List<IOccluder> ();
			dOccluders = FOWExternal.FindTargetsOfType<IOccluder> ();
			int count = texelSize * texelSize;
			occludeInfo = new float[count];
			if(dOccluderBuffer != null){
				dOccluderBuffer.Dispose();
			}
			dOccluderBuffer = new ComputeBuffer (count, 4);
			dOccluderBuffer.SetData (occludeInfo);
			Shader.SetGlobalBuffer ("_DynamicObstacle", dOccluderBuffer);
		}
		void OnRenderDynamicObstacle(){
			if (dOccluders == null) {
				return;
			}
			if (dOccluders.Count <= 0) {
				return;
			}
			// Set Data
			fowFuncCS.SetInt("_source_width",texelSize);
			fowFuncCS.SetInt("_source_height",texelSize);
			Graphics.SetRandomWriteTarget(1,dOccluderBuffer);
			fowFuncCS.SetBuffer ((int)FunctionCSKernel.Clear, "_Dest", dOccluderBuffer);
			// Clear
			fowFuncCS.Dispatch((int)FunctionCSKernel.Clear,texelSize/8,texelSize/8,1);
			// Set Dynamic Occluders Loop
			for(int i =0 ;i< dOccluders.Count;i++){
				if (dOccluders [i] == null) {
					dOccluders.RemoveAt (i);
					continue;
				}
				if (!dOccluders [i].enabled) {
					continue;
				}
				int x = 0;
				int y = 0;
				dOccluders[i].MappingToUV (this, out x, out y);
				fowFuncCS.SetInt("_ob_pos_x",x);
				fowFuncCS.SetInt("_ob_pos_y",y);
				fowFuncCS.SetInt("_ob_mask_radius",Mathf.RoundToInt(dOccluders[i].radius / areaSize * texelSize));
				fowFuncCS.SetBuffer ((int)FunctionCSKernel.RenderDynamicOccluder, "_Dest", dOccluderBuffer);
				fowFuncCS.Dispatch((int)FunctionCSKernel.RenderDynamicOccluder,texelSize/8,texelSize/8,1);
			}
			Graphics.ClearRandomWriteTargets();
			Shader.SetGlobalBuffer ("_DynamicObstacle", dOccluderBuffer);
		}
		#endregion
		
		#region  ---------------------------------------Visualize----------------------------------------
		Mesh mesh = null;
		private float ObstacleRenderCamViewLength = 10;
		private const float obstacleRenderBaise = 0.01f;
		void OnDrawGizmos(){
			Vector3 p0 = transform.TransformPoint (new Vector3 (-1, 0, -1) * areaSize * 0.5f);
			Vector3 p1 = transform.TransformPoint (new Vector3 (1, 0, -1) * areaSize * 0.5f);
			Vector3 p2 = transform.TransformPoint (new Vector3 (1, 0, 1) * areaSize * 0.5f);
			Vector3 p3 = transform.TransformPoint (new Vector3 (-1, 0, 1) * areaSize * 0.5f);

			Vector3 p4 = transform.TransformPoint (new Vector3 (-1, 0, -1) * areaSize * 0.5f + Vector3.up * ObstacleRenderCamViewLength);
			Vector3 p5 = transform.TransformPoint (new Vector3 (1, 0, -1) * areaSize * 0.5f+ Vector3.up * ObstacleRenderCamViewLength);
			Vector3 p6 = transform.TransformPoint (new Vector3 (1, 0, 1) * areaSize * 0.5f+ Vector3.up * ObstacleRenderCamViewLength);
			Vector3 p7 = transform.TransformPoint (new Vector3 (-1, 0, 1) * areaSize * 0.5f+ Vector3.up * ObstacleRenderCamViewLength);

			Gizmos.DrawLine (p0, p1);
			Gizmos.DrawLine (p2, p1);
			Gizmos.DrawLine (p2, p3);
			Gizmos.DrawLine (p0, p3);
			Gizmos.color = new Color (0.5f, 0.5f, 0.5f, 0.5f);
			Gizmos.DrawLine (p4, p5);
			Gizmos.DrawLine (p5, p6);
			Gizmos.DrawLine (p6, p7);
			Gizmos.DrawLine (p7, p4);
			if (mesh == null) {
				mesh = new Mesh ();
				List<Vector3> ver = new List<Vector3> ();
				ver.Add (transform.InverseTransformPoint(p0));
				ver.Add (transform.InverseTransformPoint(p1));
				ver.Add (transform.InverseTransformPoint(p2));
				ver.Add (transform.InverseTransformPoint(p3));
				mesh.SetVertices (ver);
				int[] tri = new int[]{ 0, 3, 2, 0, 2, 1 };
				mesh.SetTriangles(tri,0);
				mesh.RecalculateNormals ();
				mesh.RecalculateTangents ();
				ver = null;
				tri = null;
			} 

			
		}
		void OnDrawGizmosSelected(){
			Gizmos.color = new Color (1, 0, 0, 0.5f);
			Gizmos.DrawMesh (mesh,transform.position + transform.TransformPoint(Vector3.up * obstacleRenderBaise),transform.rotation);
        }
		#endregion


	}
	#region -----------------------------------------External-----------------------------------------------------
	public static class FOWExternal{
		public static void MappingToUV(this IFOWUnit unit , FogArea fogArea, out int uvX,out int uvY){
			uvX = 0;
			uvY = 0;
			Vector3 pos = fogArea.transform.InverseTransformPoint (unit.position);
			float x = (pos.x / fogArea.areaSize * 2) * 0.5f + 0.5f;
			float y = (pos.z / fogArea.areaSize * 2) * 0.5f + 0.5f;
			uvX = Mathf.RoundToInt ((x) * fogArea.texelSize);
			uvY = Mathf.RoundToInt ((y) * fogArea.texelSize);
		}
        public static void MappingToUV(Vector3 worldPos, FogArea fogArea, out int uvX, out int uvY) {
            uvX = 0;
            uvY = 0;
            Vector3 pos = fogArea.transform.InverseTransformPoint(worldPos);
            float x = (pos.x / fogArea.areaSize * 2) * 0.5f + 0.5f;
            float y = (pos.z / fogArea.areaSize * 2) * 0.5f + 0.5f;
            uvX = Mathf.RoundToInt((x) * fogArea.texelSize);
            uvY = Mathf.RoundToInt((y) * fogArea.texelSize);
        }
        /// <summary>
        /// Get texture coord(0-1) by giving world position and target fogArea
        /// </summary>
        /// <param name="worldPos"></param>
        /// <param name="fogArea"></param>
        /// <returns></returns>
        public static Vector2 WorldToFogAreaCoord(Vector3 worldPos,FogArea fogArea){
			Vector3 pos = fogArea.transform.InverseTransformPoint (worldPos);
			float x = (pos.x / fogArea.areaSize * 2) * 0.5f + 0.5f;
			float y = (pos.z / fogArea.areaSize * 2) * 0.5f + 0.5f;
			return new Vector2(x,y);
		}
		public static List<T> FindTargetsOfType<T>(){
			List<T> collect = new List<T> ();
			var all = GameObject.FindObjectsOfType<Transform> ();
			foreach (var item in all) {
				var vi = item.GetComponent<T> ();
				if (vi != null) {
					collect.Add (vi);
				}
			}
			return collect;
		}
	}


	public static class RenderTextureUtility{
		private const int CS_KERNEL_COPYCOLOR = 0;
		private static ComputeShader _loadedComputeShader;
		private static ComputeShader compute{
			get{ 
				if(_loadedComputeShader == null){
					_loadedComputeShader = Resources.Load<ComputeShader> ("FOWUtilityCS");
					UnityEngine.SceneManagement.SceneManager.activeSceneChanged += delegate{
						Resources.UnloadAsset(_loadedComputeShader);
						Debug.Log("RenderTextureUtility Data Released");
					 };
				}
				return _loadedComputeShader;
			}
		}
		public static Color[] ReadColors(this RenderTexture source,float gamma = 1.0f,float saturate = 1.0f,float brightness = 1.0f){
			int count = source.width * source.height;
			Color[] colors = new Color[count];
			ComputeBuffer buffer = new ComputeBuffer (count, 4 * 4);
			buffer.SetData (colors);
			compute.SetBuffer (CS_KERNEL_COPYCOLOR,"_Dest",buffer);
			compute.SetTexture (CS_KERNEL_COPYCOLOR,"_SourceTex2D",source);
			compute.SetFloat("_gamma",gamma);
			compute.SetFloat("_saturate",saturate);
			compute.SetFloat("_brightness",brightness);
			compute.SetInt ("_source_width", source.width);
			compute.SetInt ("_source_height", source.height);
			compute.Dispatch (CS_KERNEL_COPYCOLOR, source.width / 8, source.height / 8, 1);
			buffer.GetData (colors);
			buffer.Release ();
			buffer.Dispose ();
			buffer = null;
			return colors;
		}
	}
	#endregion
}