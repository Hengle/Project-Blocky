using Sirenix.OdinInspector;
using System;
using Cinemachine;
using FeatherWorks.Pooling;
using ProjectBlocky.UI;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	public class CameraShakeEffect : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		[BoxGroup("Settings")]
		[SerializeField]
		private float shakeIntensity = 1;

		[BoxGroup("Settings")]
		[SerializeField]
		private float shakeLength = 1;

		private float currentShakeAmount = 0;

		private CameraShakeIntensity cameraShakeController;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.cameraShakeController = CameraManager.Instance.RegisterNewController();
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Unity Update
		// ----------------------------------------------------------------------------------------------------
		private void Update() {
			if (this.currentShakeAmount > 0) {
				this.cameraShakeController.ShakeAmplitude = this.currentShakeAmount;

				this.currentShakeAmount = Math.Max(this.currentShakeAmount - Time.deltaTime * (1f / shakeLength), 0);
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Shake Methods
		// ----------------------------------------------------------------------------------------------------
		public void Shake(float shakeIntensityMultiplier = 1.0f) {
			this.currentShakeAmount = shakeIntensity * shakeIntensityMultiplier;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
