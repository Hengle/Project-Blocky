using FeatherWorks.Pooling;
using Sirenix.OdinInspector;
using System;
using ProjectBlocky.Materials;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	[RequireComponent(typeof(PlayerDamageReceiver))]
	[RequireComponent(typeof(ActorMaterialController))]
	public class HitEffect : MonoBehaviour {
        #region Fields & Properties
        // ----------------------------------------------------------------------------------------------------
        [ShowInInspector, AssetsOnly]
        [BoxGroup("Effect Settings")]
        [SerializeField]
        private GameObject effectPrefab = null;

		private int effectPrefabID;
		private FeatherPool effectPool;

		private PlayerDamageReceiver playerDamageReceiver;
		private ActorMaterialController materialController;
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

			this.playerDamageReceiver = this.GetComponent<PlayerDamageReceiver>();
			this.materialController = this.GetComponent<ActorMaterialController>();

			this.playerDamageReceiver.OnReceivedDamage += (damage) => {
				this.effectPool.Spawn(transform.position);
				this.materialController.DoHitEffect();
			};
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
