using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	[TypeInfoBox("This component acts as a distributor for collisions.")]
	public class CollisionTarget : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether this instance has hit delay.
		/// </summary>
		public bool HasHitDelay => this.hasHitDelay;
		[BoxGroup("Trigger Settings")]
		[SerializeField]
		private bool hasHitDelay = false;

		private bool isDirectionalCollision = false;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Events
		// ----------------------------------------------------------------------------------------------------
		public event Action<Collision2D> OnCollision;
		public event Action<Collision2D> OnExitCollision;
		public event Action<DirectionalCollisionInformation> OnDirectionalCollision;
		public event Action<Collision2D> OnExitDirectional;
		public event Action<ActorCollisionController> OnTriggerCollision;
		public event Action<ActorCollisionController> OnTriggerCollisionExit;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Collision Methods
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called on trigger collision.
		/// </summary>
		public void CollideTrigger(ActorCollisionController collisionController) {
			this.OnTriggerCollision?.Invoke(collisionController);
		}

		/// <summary>
		/// Collider exits the trigger.
		/// </summary>
		public void ExitTrigger(ActorCollisionController collisionController) {
			this.OnTriggerCollisionExit?.Invoke(collisionController);
		}

		/// <summary>
		/// Called on collision.
		/// </summary>
		public void Collide(Collision2D colliderHit) {
			this.OnCollision?.Invoke(colliderHit);
		}

		/// <summary>
		/// Called on collision exit.
		/// </summary>
		public void ExitCollision(Collision2D colliderHit) {
			this.OnExitCollision?.Invoke(colliderHit);
		}

		/// <summary>
		/// Called on directional collision.
		/// </summary>
		public void CollideDirectional(Collision2D colliderHit) {
			var previousCollision = this.isDirectionalCollision;
			this.isDirectionalCollision = false;
			var direction = DirectionalCollisionInformation.GetCollisionDirection(colliderHit);
			if (direction != CollisionDirection.None) {
				this.isDirectionalCollision = true;
				var collisionInfo = new DirectionalCollisionInformation(colliderHit, direction);
				this.OnDirectionalCollision?.Invoke(collisionInfo);
			}

			if (this.isDirectionalCollision == false && previousCollision == true) {
				this.OnExitDirectional(colliderHit);
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
