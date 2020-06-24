using System;
using UnityEngine;

namespace ThirdPart
{
	public interface IFOWUnit{
		Vector3 position{ get;}
	}
	public interface IVision : IFOWUnit
	{
		Vector4 getVision{get;}
		Vector4 getLastVision{get;}
		float viewDistance{ get;}
		bool NeedCalculate(FogArea fogArea,out Vector4 visionParam,out Vector4 lastVisionParam);
	}
	public interface IMaskableUnit : IFOWUnit{
		bool visible{ get;}
		void SetFogArea(FogArea area);
	}
	public interface IOccluder : IFOWUnit{
		float radius{ get;}
		bool enabled{ get;}
	}
	public interface IFogRenderer{
		void InitializeRenderer(FogArea fogArea);
	}

}

