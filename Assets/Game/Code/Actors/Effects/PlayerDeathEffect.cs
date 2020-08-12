using Sirenix.OdinInspector;
using System;
using FeatherWorks.Pooling;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	[RequireComponent(typeof(PlayerStateController))]
	public class PlayerDeathEffect : MonoBehaviour {
        #region Fields & Properties
        // ----------------------------------------------------------------------------------------------------
        [ShowInInspector, AssetsOnly]
        [BoxGroup("Effect Settings")]
        [SerializeField]
        private GameObject effectPrefab = null;
		private int effectPrefabID;

		private FeatherPool effectPool;

		private PlayerStateController playerStateController;
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

			this.playerStateController = this.GetComponent<PlayerStateController>();

			this.playerStateController.OnDeath += (dataController) => {
				this.effectPool.Spawn(transform.position);
			};
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
