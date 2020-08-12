using System;
using System.Collections.Generic;
using CreativeSpore.SuperTilemapEditor;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	public class PuzzleSwitchGroup : SerializedMonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		[BoxGroup("Puzzle Settings")]
		[ListDrawerSettings(Expanded = true, ShowIndexLabels = false), SceneObjectsOnly]
		[SerializeField]
		private List<PuzzleSwitchController> switchControllers = new List<PuzzleSwitchController>();

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

		private TriggerEventGroup triggerEventGroup;

		public bool IsActivated => this.isActivated;
		private bool isActivated = false;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Events
		// ----------------------------------------------------------------------------------------------------
		public event Action<PuzzleSwitchGroup> OnGroupActivated;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.triggerEventGroup = new TriggerEventGroup(triggerEvents);

			this.triggerEventGroup.Reset();

			foreach (var switchController in this.switchControllers) {
				switchController.OnActivationChange += (activeSwitchController) => {
					this.UpdateActivation();
				};
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Switch Methods
		// ----------------------------------------------------------------------------------------------------
		private void UpdateActivation() {
			var previousActivation = this.isActivated;
			this.isActivated = false;
			foreach (var switchController in this.switchControllers) {
				if (!switchController.IsActivated) {
					if (previousActivation == true) {
						this.triggerEventGroup.Reset();
					}
					return;
				}
			}

			if (previousActivation == false) {
				this.OnGroupActivated?.Invoke(this);
				this.triggerEventGroup.Invoke();
			}

			this.isActivated = true;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Draw Gizmos
		// ----------------------------------------------------------------------------------------------------
#if UNITY_EDITOR
		private static readonly Color GizmoColorSwitch = new Color32(63, 63, 116, 255);
		private static readonly Color GizmoColorTrigger = new Color32(50, 60, 57, 255);

		private void OnDrawGizmosSelected() {
			foreach (var switchController in this.switchControllers) {
				if (switchController != null) {
					var position = switchController.transform.position;
					var gizmoRect = new Rect(position.x - 0.5f, position.y - 0.5f, 1, 1);
					GizmosEx.DrawRect(gizmoRect.Expand(-0.01f), GizmoColorSwitch);
					GizmosEx.DrawRect(gizmoRect.Expand(-0.05f), GizmoColorSwitch);
				}
			}

			const float crossSize = 0.25f;
			foreach (var triggerEvent in this.triggerEvents) {
				if (triggerEvent != null && triggerEvent.TileObject != null) {
					var position = triggerEvent.TileObject.transform.position;
					var gizmoRect = new Rect(position.x - 0.5f, position.y - 0.5f, 1, 1);
					GizmosEx.DrawRect(gizmoRect, GizmoColorTrigger);
					GizmosEx.DrawRect(gizmoRect.Expand(0.04f), GizmoColorTrigger);
					GizmosEx.DrawRect(gizmoRect.Expand(0.08f), GizmoColorTrigger);
					Gizmos.color = GizmoColorTrigger;
					Gizmos.DrawLine(new Vector3(position.x, position.y - crossSize), new Vector3(position.x, position.y + crossSize));
					Gizmos.DrawLine(new Vector3(position.x - crossSize, position.y), new Vector3(position.x + crossSize, position.y));
				}
			}
		}
#endif
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
