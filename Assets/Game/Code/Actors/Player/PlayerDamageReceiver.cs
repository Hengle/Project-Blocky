using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	[RequireComponent(typeof(PlayerController))]
	[RequireComponent(typeof(ActorCollisionController))]
	[RequireComponent(typeof(CameraShakeEffect))]
	[TypeInfoBox("This component controls damage reception.")]
	public class PlayerDamageReceiver : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		private ActorCollisionController collisionController;
		private PlayerController playerController;
		private CameraShakeEffect cameraShakeEffect;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Events
		// ----------------------------------------------------------------------------------------------------
		public event Action<CollisionDamage> OnReceivedDamage;
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
			this.cameraShakeEffect = this.GetComponent<CameraShakeEffect>();

			this.collisionController.OnTriggerCollision += (collisionTarget) => {
				var collisionDamage = collisionTarget.GetComponent<CollisionDamage>();
				if (collisionDamage != null) {
					this.playerController.Damage(collisionDamage.Damage);
					this.OnReceivedDamage?.Invoke(collisionDamage);
					this.cameraShakeEffect.Shake();
				}
			};
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
