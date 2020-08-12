using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ProjectBlocky.UI {
	[Serializable, HideMonoScript]
	public abstract class PlayerHUDComponent : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		[ShowInInspector]
		[BoxGroup("HUD Controller"), ReadOnly]
		public PlayerHUDController PlayerHUDController {
			get {
#if UNITY_EDITOR
				if (this.playerHUDController == null) {
					this.playerHUDController = this.GetComponentInParent<PlayerHUDController>();
				}
#endif
				return this.playerHUDController;
			}
		}
		[HideInInspector]
		[SerializeField]
		protected PlayerHUDController playerHUDController;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		protected void Initialize() {
			if (this.playerHUDController == null) {
				this.playerHUDController = this.GetComponentInParent<PlayerHUDController>();
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
