using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	[TypeInfoBox("This component acts as a distributor for collisions.")]
	public class InteractionTarget : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		public bool IsActive {
			get { return this.isActive; }
			set { this.isActive = value; }
		}
		private bool isActive = true;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Events
		// ----------------------------------------------------------------------------------------------------
		public event Action<PlayerController> OnInteraction;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Interaction Methods
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Distributed an interaction event.
		/// </summary>
		public void Interact(PlayerController playerController) {
			if (this.isActive) {
				this.OnInteraction?.Invoke(playerController);
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
