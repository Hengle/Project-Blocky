using System;
using System.Collections.Generic;
using Cinemachine;
using ProjectBlocky.Actors;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectBlocky.UI {
	[Serializable, HideMonoScript]
	[TypeInfoBox("This component holds the main virtual camera.")]
	public class CameraManager : MonoBehaviour {
		#region Singleton Instance
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the HUDManager singleton instance.
		/// </summary>
		public static CameraManager Instance {
			get {
				if (!hasInstance) {
					instance = (CameraManager)FindObjectOfType(typeof(CameraManager));
					if (instance == null) {
						Debug.LogError("[CameraManager] Cannot find an instance of CameraManager!");
						return null;
					}
					hasInstance = true;
				}
				return instance;
			}
		}
		private static CameraManager instance;
		private static bool hasInstance = false;

		private List<CameraShakeIntensity> cameraShakeControllers = new List<CameraShakeIntensity>();
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the virtual camera.
		/// </summary>
		public CinemachineVirtualCamera VirtualCamera {
			get {
				if (this.virtualCamera == null) {
					this.virtualCamera = this.GetComponent<CinemachineVirtualCamera>();
				}
				return this.virtualCamera;
			}
		}
		private CinemachineVirtualCamera virtualCamera;

		/// <summary>
		/// Gets the camera noise controller.
		/// </summary>
		public CinemachineBasicMultiChannelPerlin CameraNoise {
			get {
				if (this.cameraNoise == null) {
					this.cameraNoise = VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
				}
				return this.cameraNoise;
			}
		}
		private CinemachineBasicMultiChannelPerlin cameraNoise;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Unity Update
		// ----------------------------------------------------------------------------------------------------
		private void LateUpdate() {
			var cameraNoiseController = this.CameraNoise;

			var shakeControllerCount = this.cameraShakeControllers.Count;

			float amplitudeIntensity = 0;
			float frequencyIntensity = 1;
			for (int i = 0; i < shakeControllerCount; i++) {
				var shakeIntensityController = this.cameraShakeControllers[i];
				amplitudeIntensity += shakeIntensityController.ShakeAmplitude;
				frequencyIntensity += shakeIntensityController.ShakeFrequency;
			}

			cameraNoiseController.m_AmplitudeGain = amplitudeIntensity;
			cameraNoiseController.m_FrequencyGain = frequencyIntensity;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Camera Shake Methods
		// ----------------------------------------------------------------------------------------------------
		public CameraShakeIntensity RegisterNewController() {
			var newController = new CameraShakeIntensity();
			this.cameraShakeControllers.Add(newController);
			return newController;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
