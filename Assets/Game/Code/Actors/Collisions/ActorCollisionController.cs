using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	public class ActorCollisionController : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		[BoxGroup("Collision Settings")]
		[SerializeField]
		private bool useHitDelay = true;

		[BoxGroup("Collision Settings"), ShowIf("useHitDelay")]
		[MinValue(0.25)]
		[LabelText("Hit Delay")]
		[SerializeField]
		private float collisionHitDelay = 1;
		private float currentHitDelay = 0;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Events
		// ----------------------------------------------------------------------------------------------------
		public event Action<CollisionTarget> OnTriggerCollision;
		public event Action<CollisionTarget> OnTriggerCollisionExit;
		public event Action<CollisionTarget> OnCollision;
		public event Action<CollisionTarget> OnCollisionExit;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Collision Handling
		// ----------------------------------------------------------------------------------------------------
		private void OnTriggerStay2D(Collider2D other) {
			var collisionTarget = other.GetComponent<CollisionTarget>();
			if (collisionTarget != null) {
				if (this.useHitDelay && collisionTarget.HasHitDelay) {
					if (this.currentHitDelay > 0) {
						return;
					}
					this.currentHitDelay = this.collisionHitDelay;
				}
				this.OnTriggerCollision?.Invoke(collisionTarget);
				collisionTarget.CollideTrigger(this);
			}
		}

		private void OnTriggerExit2D(Collider2D other) {
			var collisionTarget = other.GetComponent<CollisionTarget>();
			if (collisionTarget != null) {
				this.OnTriggerCollisionExit?.Invoke(collisionTarget);
				collisionTarget.ExitTrigger(this);
			}
		}

		private void OnCollisionEnter2D(Collision2D collisionInfo) {
			var collisionTarget = collisionInfo.gameObject.GetComponent<CollisionTarget>();
			if (collisionTarget != null) {
				this.OnCollision?.Invoke(collisionTarget);
				collisionTarget.Collide(collisionInfo);
			}
		}

		private void OnCollisionStay2D(Collision2D collisionInfo) {
			var collisionTarget = collisionInfo.gameObject.GetComponent<CollisionTarget>();
			if (collisionTarget != null) {
				this.OnCollision?.Invoke(collisionTarget);
				collisionTarget.CollideDirectional(collisionInfo);
			}
		}

		private void OnCollisionExit2D(Collision2D collisionInfo) {
			var collisionTarget = collisionInfo.gameObject.GetComponent<CollisionTarget>();
			if (collisionTarget != null) {
				this.OnCollisionExit?.Invoke(collisionTarget);
				collisionTarget.ExitCollision(collisionInfo);
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Unity Update
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Fixeds the update.
		/// </summary>
		private void FixedUpdate() {
			if (this.currentHitDelay >= 0) {
				this.currentHitDelay = Mathf.Max(this.currentHitDelay - Time.fixedDeltaTime, 0);
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
