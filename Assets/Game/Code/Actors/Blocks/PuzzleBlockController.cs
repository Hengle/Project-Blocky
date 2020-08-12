using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	public class PuzzleBlockController : MonoBehaviour {
        #region Fields & Properties
        // ----------------------------------------------------------------------------------------------------
        [BoxGroup("Settings"), Required]
        [SerializeField]
        private SpriteRenderer stateSpriteRenderer = null;

		public bool IsActivated {
			get { return this.isActivated; }
			set {
				this.isActivated = value;
				this.stateSpriteRenderer.enabled = this.isActivated;
			}
		}
		private bool isActivated = false;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Block Methods
		// ----------------------------------------------------------------------------------------------------
		public void SetActivation(bool isActive) {
			this.IsActivated = isActive;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
