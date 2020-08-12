using Sirenix.OdinInspector;
using System;
using System.Collections;
using ProjectBlocky.GameFlow;
using UnityEngine;

namespace ProjectBlocky.Actors {
	[Serializable, HideMonoScript]
	[RequireComponent(typeof(CircleCollider2D))]
	[RequireComponent(typeof(SpriteRenderer))]
	public class PlayerStateController : MonoBehaviour {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		[BoxGroup("Respawn Settings")]
		[MinValue(0)]
		[SerializeField]
		private float respawnTime = 2;

		private float currentRespawnTime = 0;
		private float currentSpawnTime = 0;


		/// <summary>
		/// Gets a value indicating whether this instance is alive.
		/// </summary>
		public bool IsAlive {
			get { return this.isAlive; }
		}
		private bool isAlive = true;

		/// <summary>
		/// Gets a value indicating whether this player is spawning.
		/// </summary>
		public bool IsRespawning {
			get { return this.isRespawning; }
		}
		private bool isRespawning = false;

		/// <summary>
		/// Gets a value indicating whether this player is spawning.
		/// </summary>
		public bool IsSpawning {
			get { return this.isSpawning; }
		}
		private bool isSpawning = false;

		private PlayerController playerController;

		private Collider2D playerCollider;
		private SpriteRenderer spriteRenderer;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Static Fields
		// ----------------------------------------------------------------------------------------------------
		private static readonly WaitForSeconds TeleportCoroutine = new WaitForSeconds(0.85f);
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Events
		// ----------------------------------------------------------------------------------------------------
		public event Action<PlayerStateController, SpawnData> OnSpawn;
		public event Action<PlayerStateController, SpawnData> OnRespawn;

		public struct SpawnData {
			public bool IsSilentSpawn;
			public float SpawnEffectMultiplier;

			public SpawnData(float spawnEffectMultiplier = 1.0f, bool isSilentSpawn = true) {
				this.SpawnEffectMultiplier = spawnEffectMultiplier;
				this.IsSilentSpawn = isSilentSpawn;
			}
		}

		public event Action<PlayerStateController> OnDisable;
		public event Action<PlayerStateController> OnDeath;
		public event Action<PlayerStateController, Transform> OnTeleport;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Called upon awaking this behavior.
		/// </summary>
		private void Awake() {
			this.playerController = this.GetComponent<PlayerController>();
			this.playerCollider = this.GetComponent<Collider2D>();
			this.spriteRenderer = this.GetComponent<SpriteRenderer>();

			this.playerController.Health.OnHealthChanged += (healthComponent) => {
				if (healthComponent.Health > 0) {

				}
				else {
					if (this.isAlive) {
						this.OnDeath?.Invoke(this);
					}
				}
			};

			this.OnDeath += (pc) => {
				this.Respawn(this.respawnTime);
			};
		}

		public void Disable(bool invokeEvent = true) {
			this.isAlive = false;
			this.playerCollider.enabled = false;
			this.isRespawning = false;
			this.isSpawning = false;

			if (invokeEvent) {
				this.OnDisable?.Invoke(this);
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Spawning / Respawning
		// ----------------------------------------------------------------------------------------------------
		public void Respawn(float respawnDelay) {
			this.isAlive = false;
			this.playerCollider.enabled = false;
			this.currentRespawnTime = respawnDelay;
			this.isRespawning = true;
			this.OnRespawn?.Invoke(this, new SpawnData());
		}

		public void Spawn(Vector2 spawnPosition, float spawnTime = 1.0f, bool doSpawnEffect = true) {
			this.transform.position = spawnPosition;

			this.playerController.ResetHealth();
			this.currentSpawnTime = 1.5f * (1f / spawnTime);
			this.isSpawning = true;
			this.OnSpawn?.Invoke(this, new SpawnData(spawnTime, doSpawnEffect));
		}

		private void OnSpawned() {
			this.isAlive = true;
			this.playerCollider.enabled = true;
		}

		public void TeleportAway(Transform targetTransform) {
			this.Disable(false);
			this.OnTeleport?.Invoke(this, targetTransform);
		}

		public IEnumerator TeleportTo(Transform targetTransform) {
			TeleportAway(targetTransform);

			yield return TeleportCoroutine;

			Spawn(targetTransform.position, 1.5f, false);
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Unity Update
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Updates this instance.
		/// </summary>
		private void Update() {
			if (isRespawning) {
				if (currentRespawnTime > 0) {
					this.currentRespawnTime = Math.Max(this.currentRespawnTime - Time.deltaTime, 0);
				}
				else {
					this.isRespawning = false;

					this.Spawn(LevelManager.Instance.ActiveSection.GetSpawnPosition(this.playerController.PlayerType));
				}
			}

			if (isSpawning) {
				if (currentSpawnTime > 0) {
					this.currentSpawnTime = Math.Max(this.currentSpawnTime - Time.deltaTime, 0);
				}
				else {
					this.isSpawning = false;
					this.OnSpawned();
				}
			}
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
