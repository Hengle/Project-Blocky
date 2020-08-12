using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	[RequireComponent(typeof(InputController))]
	[RequireComponent(typeof(ActorCollisionController))]
	[TypeInfoBox("This component controls the player's interaction.")]
	public class InteractionController : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		private InputController inputController;
		private PlayerController playerController;

		private ActorCollisionController collisionController;

		private InteractionTarget interactionTarget;
		private bool hasInteractionTarget = false;

		/// <summary>
		/// Gets a value indicating whether this instance can interact.
		/// </summary>
		public bool CanInteract => this.canInteract;
		private bool canInteract = false;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Events
		// ----------------------------------------------------------------------------------------------------
		public event Action<PlayerController, InteractionTarget> OnInteract;
		public event Action<InteractionController> OnInteractabilityChanged;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.inputController = this.GetComponent<InputController>();
			this.playerController = this.GetComponent<PlayerController>();

			this.collisionController = this.GetComponent<ActorCollisionController>();
			this.collisionController.OnTriggerCollision += (collisionTarget) => {
				var target = collisionTarget.GetComponent<InteractionTarget>();
				if (target != null) {
					this.interactionTarget = target;
					this.hasInteractionTarget = target.IsActive;
				}
			};

			this.collisionController.OnTriggerCollisionExit += (collisionTarget) => {
				var target = collisionTarget.GetComponent<InteractionTarget>();
				if (this.interactionTarget == target) {
					this.interactionTarget = null;
					this.hasInteractionTarget = false;
				}
			};
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Unity Update
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Updates this instance.
		/// </summary>
		private void FixedUpdate() {
			var previousCanInteract = this.canInteract;

			this.canInteract = false;
			if (hasInteractionTarget) {
				if (CollisionUtils.DistanceToTile(this.transform.position, this.interactionTarget.transform.position) < 0.05f) {
					this.canInteract = true;
					if (this.inputController.IsInteracting) {
						this.OnInteract?.Invoke(this.playerController, this.interactionTarget);
						this.interactionTarget.Interact(this.playerController);
					}
				}
			}

			if (this.canInteract != previousCanInteract) {
				this.OnInteractabilityChanged?.Invoke(this);
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
