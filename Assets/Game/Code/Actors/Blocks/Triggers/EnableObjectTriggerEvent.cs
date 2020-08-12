using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	public class EnableObjectTriggerEvent : BaseTriggerEvent {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the tile object.
		/// </summary>
		public override GameObject TileObject => tileObject;

        [LabelText("Tile Object")]
        [SerializeField, Required]
        private GameObject tileObject = null;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Trigger Methods
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Triggers this event.
		/// </summary>
		public override void Trigger() {
			this.tileObject.SetActive(true);
		}

		/// <summary>
		/// Resets the state.
		/// </summary>
		public override void ResetState() {
			this.tileObject.SetActive(false);
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
