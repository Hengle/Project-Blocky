using Sirenix.OdinInspector;
using System;
using FeatherWorks.Pooling;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	[RequireComponent(typeof(CollisionTarget))]
	public class PickupEffect : MonoBehaviour {
        #region Fields & Properties
        // ----------------------------------------------------------------------------------------------------
        [ShowInInspector, AssetsOnly]
        [BoxGroup("Effect Settings")]
        [SerializeField]
        private GameObject effectPrefab = null;
		private int effectPrefabID;

		//[BoxGroup("Effect Settings")]
		//[SerializeField]
		//private bool disableOnPickup = true;

		private FeatherPool effectPool;

		private CollisionTarget collisionTarget;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.effectPrefabID = effectPrefab.GetInstanceID();
			this.effectPool = FeatherPoolManager.Instance.GetPool(this.effectPrefabID);

			this.collisionTarget = this.GetComponent<CollisionTarget>();

			this.collisionTarget.OnTriggerCollision += (collisionController) => {
				this.effectPool.Spawn(transform.position);
				this.gameObject.SetActive(false);
			};
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
