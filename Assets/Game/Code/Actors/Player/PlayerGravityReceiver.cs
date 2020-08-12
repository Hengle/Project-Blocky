using FeatherWorks;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	[RequireComponent(typeof(ActorCollisionController))]
	[TypeInfoBox("This component allows the object to be affected by gravitational pulls based on the CollisionGravity component.")]
	public class PlayerGravityReceiver : MonoBehaviour, IResettable {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		private MovementController movementController;
		private ActorCollisionController collisionController;

		private CollisionGravity gravitationalObject;
		private Transform gravicationalTransform;
		private bool hasGravitationalObject = false;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.movementController = this.GetComponent<MovementController>();
			this.collisionController = this.GetComponent<ActorCollisionController>();

			this.hasGravitationalObject = false;

			this.collisionController.OnTriggerCollision += (collision) => {
				var collisionGravity = collision.GetComponent<CollisionGravity>();
				if (collisionGravity != null) {
					this.gravitationalObject = collisionGravity;
					this.gravicationalTransform = collisionGravity.transform;
					this.hasGravitationalObject = collisionGravity.IsActive;
				}
			};

			this.collisionController.OnTriggerCollisionExit += (collision) => {
				if (this.hasGravitationalObject) {
					var collisionGravity = collision.GetComponent<CollisionGravity>();
					if (this.gravitationalObject == collisionGravity) {
						this.gravitationalObject = null;
						this.gravicationalTransform = null;
						this.hasGravitationalObject = false;
					}
				}
			};
		}

		/// <summary>
		/// Resets the state.
		/// </summary>
		public void ResetState() {
			this.gravitationalObject = null;
			this.hasGravitationalObject = false;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Unity Update
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Fixeds the update.
		/// </summary>
		private void FixedUpdate() {
			if (this.hasGravitationalObject) {
				this.movementController.AddForceTowardsPointWithSpeed(this.gravicationalTransform.position, this.gravitationalObject.Gravity);
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
