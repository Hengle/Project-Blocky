using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	[RequireComponent(typeof(InteractionTarget))]
	public class TeleporterBlockController : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		[BoxGroup("Settings")]
		[ShowInInspector, Required]
		public TeleporterBlockController TargetTeleporter {
			get { return this.targetTeleporter; }
			set {
				this.targetTeleporter = value;
				if (this.targetTeleporter && this.targetTeleporter.TargetTeleporter != this) {
					this.targetTeleporter.TargetTeleporter = this;
				}
			}
		}
		[SerializeField, HideInInspector]
		private TeleporterBlockController targetTeleporter;

		private InteractionTarget interactionTarget;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Events
		// ----------------------------------------------------------------------------------------------------
		public event Action<TeleporterBlockController> OnTeleport;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.interactionTarget = this.GetComponent<InteractionTarget>();

			this.interactionTarget.OnInteraction += (playerController) => {
				var playerStateController = playerController.StateController;
				if (CollisionUtils.IsObjectInsideTile(transform.position, playerStateController.transform.position)) {
					StartCoroutine(playerStateController.TeleportTo(targetTeleporter.transform));
					this.OnTeleport?.Invoke(this);
				}
			};
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Drawing Gizmos
		// ----------------------------------------------------------------------------------------------------
#if UNITY_EDITOR
		private static readonly Color GizmoColor = new Color32(90, 110, 225, 255);

		private void OnDrawGizmosSelected() {
			if (this.targetTeleporter != null) {
				Gizmos.color = GizmoColor;
				Gizmos.DrawLine(this.transform.position, this.targetTeleporter.transform.position);
			}
		}
#endif
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
