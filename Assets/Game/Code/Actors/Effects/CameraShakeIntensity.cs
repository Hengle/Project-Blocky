using Sirenix.OdinInspector;
using System;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	public class CameraShakeIntensity {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		public float ShakeAmplitude = 0f;
		public float ShakeFrequency = 0f;
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
