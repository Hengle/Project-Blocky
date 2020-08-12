using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	[RequireComponent(typeof(PlayerController))]
	[RequireComponent(typeof(ActorCollisionController))]
	[TypeInfoBox("This component controls pickup reception.")]
	public class PlayerPickupReceiver : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		private ActorCollisionController collisionController;
		private PlayerController playerController;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.playerController = this.GetComponent<PlayerController>();
			this.collisionController = this.GetComponent<ActorCollisionController>();

			this.collisionController.OnTriggerCollision += (collisionTarget) => {
				var collisionPickup = collisionTarget.GetComponent<CollisionPickup>();
				if (collisionPickup != null) {
					this.playerController.AddScore(collisionPickup.Score);
				}
			};
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
