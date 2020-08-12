using Sirenix.OdinInspector;
using System;
using Unity.Mathematics;
using UnityEngine;

namespace ProjectBlocky.Materials {
	[Serializable, HideMonoScript]
	[TypeInfoBox("This component controls the Actor Material shader.")]
	public class ActorMaterialController : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		private static readonly int HitAmountProperty = Shader.PropertyToID("_HitAmount");
		private static readonly int FadeAmountProperty = Shader.PropertyToID("_FadeAmount");

		private Material material;

		private float hitFade = 0;
		private float spawnFade = 0;
		private bool isSpawning = false;
		private bool isDespawning = false;

		private float spawnMultiplier = 1.0f;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.material = this.GetComponent<SpriteRenderer>().material;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Effect Methods
		// ----------------------------------------------------------------------------------------------------
		public void DoSpawnEffect(float multiplier = 1.0f) {
			this.spawnFade = 0;
			this.spawnMultiplier = multiplier;
			this.isSpawning = true;
			this.isDespawning = false;

			UpdateMaterial();
		}

		public void DoDespawnEffect(float multiplier = 1.0f) {
			this.spawnFade = 1;
			this.spawnMultiplier = multiplier;
			this.isSpawning = false;
			this.isDespawning = true;

			UpdateMaterial();
		}

		public void DoHitEffect() {
			this.hitFade = 1;

			UpdateMaterial();
		}

		public void PrepareSpawn() {
			this.spawnFade = 0;

			UpdateMaterial();
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Unity Update
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Updates this instance.
		/// </summary>
		private void Update() {
			if (this.hitFade > 0) {
				this.hitFade = math.max(this.hitFade - (Time.deltaTime * 4), 0);
			}

			if (this.isSpawning) {
				if (this.spawnFade < 1) {
					this.spawnFade = math.min(this.spawnFade + ((Time.deltaTime / 1.6f) * this.spawnMultiplier), 1);
				}
				else {
					this.isSpawning = false;
				}
			}else if (this.isDespawning) {
				if (this.spawnFade > 0) {
					this.spawnFade = math.max(this.spawnFade - (Time.deltaTime * 1.2f * this.spawnMultiplier), 0);
				}
				else {
					this.isDespawning = false;
				}
			}

			UpdateMaterial();
		}

		private void UpdateMaterial() {
			this.material.SetFloat(FadeAmountProperty, this.spawnFade);
			this.material.SetFloat(HitAmountProperty, this.hitFade);
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
