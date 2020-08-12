using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(InteractionController))]
	[TypeInfoBox("This component controls the player's animations.")]
	public class PlayerAnimationController : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		private static readonly int ShineTrigger = Animator.StringToHash("Shine");

		private InteractionController interactionController;

		private Animator animator;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.animator = this.GetComponent<Animator>();

			this.interactionController = this.GetComponent<InteractionController>();
			this.interactionController.OnInteractabilityChanged += (ic) => {
				this.SetShine(ic.CanInteract);
			};
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Animation Methods
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Does the shine animation.
		/// </summary>
		public void SetShine(bool shineEnabled) {
			this.animator.SetBool(ShineTrigger, shineEnabled);
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
