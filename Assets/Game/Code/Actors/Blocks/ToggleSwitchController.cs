using System;
using System.Collections.Generic;
using ProjectBlocky.Materials;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	[RequireComponent(typeof(InteractionTarget))]
	[RequireComponent(typeof(CameraShakeEffect))]
	public class ToggleSwitchController : SerializedMonoBehaviour {
		#region Constants
		// ----------------------------------------------------------------------------------------------------
		private const float InteractionDelay = 0.5f;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		[BoxGroup("Settings")]
		[SerializeField]
		private bool activatedByDefault = false;
		private bool isActivated = false;

		[BoxGroup("Settings")]
		[SerializeField]
		private bool isOneWay = true;

        [BoxGroup("Material Settings")]
        [SerializeField, Required]
        private Material materialOnState = null;
		[BoxGroup("Material Settings")]
		[SerializeField, Required]
		private Material materialOffState = null;
        [BoxGroup("Material Settings")]
        [SerializeField, Required]
        private SpriteRenderer statusSprite = null;

		[ShowInInspector]
		[ListDrawerSettings(Expanded = true, ShowIndexLabels = false)]
		[SerializeField, ValidateInput("ValidateTriggerEvents")]
		private List<IBlockTriggerEvent> triggerEvents = new List<IBlockTriggerEvent>();

#if UNITY_EDITOR
		private bool ValidateTriggerEvents(List<IBlockTriggerEvent> triggerEventList) {
			if (triggerEventList == null) { triggerEventList = new List<IBlockTriggerEvent>(); }

			foreach (var triggerEvent in triggerEventList) {
				triggerEvent.Parent = this.gameObject;
			}

			return true;
		}
#endif

		private bool CanBeToggled => !(isOneWay && isActivated != activatedByDefault);

		private TriggerEventGroup triggerEventGroup;
		private InteractionTarget interactionTarget;
		private CameraShakeEffect cameraShakeEffect;
		private HighlightMaterialController highlightMaterial;
		private CollisionGravity collisionGravity;

		private float interactionDelay = 0;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Events
		// ----------------------------------------------------------------------------------------------------
		public event Action<ToggleSwitchController> OnStatusChanged;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.triggerEventGroup = new TriggerEventGroup(triggerEvents);

			this.interactionTarget = this.GetComponent<InteractionTarget>();
			this.cameraShakeEffect = this.GetComponent<CameraShakeEffect>();
			this.highlightMaterial = this.GetComponent<HighlightMaterialController>();
			this.collisionGravity = this.GetComponent<CollisionGravity>();

			this.interactionTarget.OnInteraction += (playerController) => {
				var playerStateController = playerController.StateController;
				if (CollisionUtils.IsObjectInsideTile(transform.position, playerStateController.transform.position)) {
					this.Toggle();
				}
			};

			this.OnStatusChanged += (toggleSwitch) => {
				if (toggleSwitch.isActivated) {
					this.triggerEventGroup.Invoke();
				}
				else {
					this.triggerEventGroup.Reset();
				}
			};

			this.SetActive(this.activatedByDefault, true);
		}


		private void OnEnable() {
			this.interactionDelay = 0;
		}

		public void SetActive(bool isActive = false, bool isForced = false) {
			var previousState = this.isActivated;

			this.isActivated = isActive;

			if (isForced || this.isActivated != previousState) {
				this.statusSprite.material = this.isActivated ? this.materialOnState : this.materialOffState;
				this.OnStatusChanged?.Invoke(this);

				if (!CanBeToggled) {
					this.highlightMaterial.SetOverrideValue(0);
					this.interactionTarget.IsActive = false;
					this.collisionGravity.IsActive = false;
				}
			}
		}

		public void Toggle() {
			if (this.interactionDelay <= 0) {
				if (!CanBeToggled) {
					return;
				}
				this.SetActive(!this.isActivated);

				this.cameraShakeEffect.Shake();
				this.interactionDelay = InteractionDelay;
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Unity Update
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Fixeds the update.
		/// </summary>
		private void FixedUpdate() {
			if (this.interactionDelay > 0) {
				this.interactionDelay = math.max(interactionDelay - Time.deltaTime, 0);
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
