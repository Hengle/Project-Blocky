using Sirenix.OdinInspector;
using System;
using CreativeSpore.SuperTilemapEditor;
using Sirenix.Serialization;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	public partial class EnableLayerTriggerEvent : BaseTriggerEvent {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the tile object.
		/// </summary>
		public override GameObject TileObject => tileObject;

        [LabelText("Tilemap Layer On")]
        [ValueDropdown("GetLayers")]
        [SerializeField, Required]
        private GameObject tileObject = null;

		[SerializeField]
		[PropertyTooltip("Whether or not to use a separate layer for its deactivated state")]
		private bool hasDeactivatedState = false;

        [ShowIf("hasDeactivatedState")]
        [LabelText("Tilemap Layer Off")]
        [ValueDropdown("GetLayers")]
        [SerializeField, Required]
        private GameObject tileObjectOff = null;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Trigger Methods
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Triggers this event.
		/// </summary>
		public override void Trigger() {
			this.tileObject.SetActive(true);
			if (hasDeactivatedState) {
				this.tileObjectOff.SetActive(false);
			}
		}

		/// <summary>
		/// Resets the state.
		/// </summary>
		public override void ResetState() {
			this.tileObject.SetActive(false);
			if (hasDeactivatedState) {
				this.tileObjectOff.SetActive(true);
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
