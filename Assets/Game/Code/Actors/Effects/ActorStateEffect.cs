using Sirenix.OdinInspector;
using System;
using FeatherWorks.Pooling;
using ProjectBlocky.Materials;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	[RequireComponent(typeof(PlayerStateController))]
	[RequireComponent(typeof(ActorMaterialController))]
	public class ActorStateEffect : MonoBehaviour {
        #region Fields & Properties
        // ----------------------------------------------------------------------------------------------------
        [BoxGroup("Effect Settings"), Required, AssetsOnly]
        [SerializeField]
        private GameObject spawnEffectPrefab = null;
		private int spawnEffectPrefabID;

        [BoxGroup("Effect Settings"), Required, AssetsOnly]
        [SerializeField]
        private GameObject teleportEffectPrefab = null;
		private int teleportEffectPrefabID;

		private FeatherPool spawnEffectPool;
		private FeatherPool teleportEffectPool;

		private PlayerStateController playerStateController;
		private ActorMaterialController materialController;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.spawnEffectPrefabID = spawnEffectPrefab.GetInstanceID();
			this.spawnEffectPool = FeatherPoolManager.Instance.GetPool(this.spawnEffectPrefabID);

			this.teleportEffectPrefabID = teleportEffectPrefab.GetInstanceID();
			this.teleportEffectPool = FeatherPoolManager.Instance.GetPool(this.teleportEffectPrefabID);

			this.playerStateController = this.GetComponent<PlayerStateController>();
			this.materialController = this.GetComponent<ActorMaterialController>();

			this.playerStateController.OnDisable += (dataController) => {
				this.materialController.PrepareSpawn();
			};

			this.playerStateController.OnRespawn += (dataController, spawnData) => {
				this.materialController.PrepareSpawn();
			};

			this.playerStateController.OnSpawn += (dataController, spawnData) => {
				if (spawnData.IsSilentSpawn) {
					this.spawnEffectPool.Spawn(transform.position);
				}
				this.materialController.DoSpawnEffect(spawnData.SpawnEffectMultiplier);
			};

			this.playerStateController.OnTeleport += (dataController, targetTransform) => {
				var effectObject = this.teleportEffectPool.Spawn(transform.position);
				effectObject.GetComponent<ParticleAttractor>().TargetTransform = targetTransform;
				this.materialController.DoDespawnEffect();
			};
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
